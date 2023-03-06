using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TrafficGame.Scripts.Level.CarsSpawn.Car;
using TrafficGame.Scripts.Reusable;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TrafficGame.Scripts.Level.CarsSpawn
{
    public class CarsSpawnController : MonoBehaviour
    {
        [SerializeField] private List<CarRoute> _carRoutes;
        [SerializeField] private AssetReferenceGameObject _carViewReference;
        [SerializeField] private int _spawnDelayMillis = 1000;
        [SerializeField] private int _minCarSpeed = 100;
        [SerializeField] private int _maxCarSpeed = 200;
        
        private readonly HashSet<CarPresenter> _carPresenters = new();
        
        private CancellationTokenSource _spawningCts;
        private Transform _transform;

        public event Action<CarModel> CarSpawned;
        public event Action<CarModel> CarRemoved; 

        private void Awake()
        {
            _transform = transform;
        }
        
        public async UniTask SpawnFromModels(IEnumerable<CarModel> carModels, CancellationToken cancellationToken)
        {
            foreach (var carModel in carModels)
            {
                if(cancellationToken.IsCancellationRequested) return;
                await SpawnCar(carModel, cancellationToken);
            }
        }
        
        public void EnableRandomSpawning(bool enable)
        {
            _spawningCts?.Cancel();
            
            if (enable)
            {
                _spawningCts = new CancellationTokenSource();
                RandomSpawningProcess(_spawningCts.Token).Forget();
            }
            else
            {
                _spawningCts?.Cancel();
            }
        }

        public void RemoveCar(CarPresenter car)
        {
            car.Dispose();
            _carPresenters.Remove(car);
            
            CarRemoved?.Invoke(car.CarModel);
        }

        private async UniTask RandomSpawningProcess(CancellationToken cancellationToken)
        {
            var random = new System.Random();

            while (!cancellationToken.IsCancellationRequested)
            {
                var routeIndex = random.Next(_carRoutes.Count);
                var randomRoute = _carRoutes.ElementAt(routeIndex);

                // Spawn only if no other objects in spawn area
                if (randomRoute.spawnArea.ObjectsInVolume.Count == 0)
                {
                    var randomSpeed = random.Next(_minCarSpeed, _maxCarSpeed);
                    
                    await SpawnCar(new CarModel(randomRoute.startPoint.position, randomRoute.startPoint.rotation, routeIndex, randomSpeed),
                        cancellationToken);
                }

                if(cancellationToken.IsCancellationRequested) return;

                await UniTask.Delay(_spawnDelayMillis, DelayType.DeltaTime, cancellationToken: cancellationToken);
            }
        }
        
        private async UniTask SpawnCar(CarModel carModel, CancellationToken cancellationToken)
        {
            var carPresenter = await CarPresenter.Initialize(_carViewReference, carModel);

            if (cancellationToken.IsCancellationRequested)
            {
                carPresenter.Dispose();
            }
            else
            {
                carPresenter.SetParent(_transform);
                carPresenter.StartMoving();
                _carPresenters.Add(carPresenter);
                
                CarSpawned?.Invoke(carModel);
            }
        }

        private void OnDestroy()
        {
            EnableRandomSpawning(false);
            foreach (var carPresenter in _carPresenters)
            {
                carPresenter.Dispose();
            }

            CarSpawned = null;
            CarRemoved = null;
        }

        [Serializable]
        public class CarRoute
        {
            public EventTriggerVolume spawnArea;
            public Transform startPoint;
        }
    }
}