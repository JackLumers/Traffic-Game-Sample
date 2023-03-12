using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TrafficGame.Scripts.EventTriggerVolumes;
using TrafficGame.Scripts.Level.CarsSpawn.Car;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TrafficGame.Scripts.Level.CarsSpawn
{
    public class CarSpawnVolume : BaseEventTriggerVolume<CarSpawnVolume>
    {
        [SerializeField] private int _index;
        [SerializeField] private CarsSpawningConfig _carsSpawningConfig;

        private Transform _spawnParentTransform;
        private Transform _transform;
        private CancellationTokenSource _cts;
        
        public event Action<CarPresenter> CarSpawned;
        
        private void Awake()
        {
            _transform = transform;
        }

        public void StartRandomCarsSpawn(Transform spawnParentTransform, CancellationToken cancellationToken)
        {
            _spawnParentTransform = spawnParentTransform;
            
            _cts?.Cancel();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(), cancellationToken);
            
            RandomSpawningProcess(_cts.Token).Forget();
        }

        public async UniTask SpawnCar(CarModel carModel, CancellationToken cancellationToken)
        {
            var carPresenter = await CarPresenter.Initialize(_carsSpawningConfig.CarViewReference, carModel);

            if (cancellationToken.IsCancellationRequested)
            {
                carPresenter.Dispose();
            }
            else
            {
                carPresenter.SetParent(_spawnParentTransform);
                carPresenter.StartMoving();
                
                CarSpawned?.Invoke(carPresenter);
            }
        }
        
        private async UniTask RandomSpawningProcess(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var delay = Random.Range(_carsSpawningConfig.MinSpawnDelay, _carsSpawningConfig.MaxSpawnDelay);
                
                await UniTask.Delay(delay, DelayType.DeltaTime, cancellationToken: cancellationToken);
                
                if(cancellationToken.IsCancellationRequested) return;
                
                // Spawn only if no other objects in spawn area
                if (ObjectsInVolume.Count == 0)
                {
                    var speed = Random.Range(_carsSpawningConfig.MinCarSpeed, _carsSpawningConfig.MaxCarSpeed);
                    
                    await SpawnCar(new CarModel(_transform.position, _transform.rotation, _index, speed),
                        cancellationToken);
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CarSpawned = null;
        }
    }
}