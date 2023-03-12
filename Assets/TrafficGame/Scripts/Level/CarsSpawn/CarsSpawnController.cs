using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TrafficGame.Scripts.Level.CarsSpawn.Car;
using UnityEngine;

namespace TrafficGame.Scripts.Level.CarsSpawn
{
    public class CarsSpawnController : MonoBehaviour
    {
        [SerializeField] private List<CarSpawnVolume> _carSpawnAreas;

        private readonly HashSet<CarPresenter> _carPresenters = new();
        
        private CancellationTokenSource _spawningCts;
        private Transform _transform;

        public event Action<CarModel> CarSpawned;
        public event Action<CarModel> CarRemoved;

        private void Awake()
        {
            _transform = transform;

            foreach (var carSpawnArea in _carSpawnAreas)
            {
                carSpawnArea.CarSpawned += OnCarSpawned;
            }
        }

        public async UniTask SpawnFromModels(IEnumerable<CarModel> carModels, CancellationToken cancellationToken)
        {
            foreach (var carModel in carModels)
            {
                if(cancellationToken.IsCancellationRequested) return;

                await _carSpawnAreas[carModel.RoadIndex].SpawnCar(carModel, cancellationToken);
            }
        }
        
        public void EnableRandomSpawning(bool enable)
        {
            _spawningCts?.Cancel();

            if (enable)
            {
                _spawningCts = new CancellationTokenSource();

                foreach (var carSpawnVolume in _carSpawnAreas)
                {
                    carSpawnVolume.StartRandomCarsSpawn(_transform, _spawningCts.Token);
                }
            }
        }

        public void RemoveCar(CarPresenter car)
        {
            car.Dispose();
            _carPresenters.Remove(car);
            
            CarRemoved?.Invoke(car.CarModel);
        }

        private void OnCarSpawned(CarPresenter carPresenter)
        {
            _carPresenters.Add(carPresenter);
            
            CarSpawned?.Invoke(carPresenter.CarModel);
        }
        
        private void OnDestroy()
        {
            _spawningCts?.Cancel();
            foreach (var carPresenter in _carPresenters)
            {
                carPresenter.Dispose();
            }

            CarSpawned = null;
            CarRemoved = null;
        }
    }
}