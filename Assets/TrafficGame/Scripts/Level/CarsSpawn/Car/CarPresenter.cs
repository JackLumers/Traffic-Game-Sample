using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TrafficGame.Scripts.Level.TrafficLight;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TrafficGame.Scripts.Level.CarsSpawn.Car
{
    public class CarPresenter : IDisposable
    {
        private readonly AsyncOperationHandle<GameObject> _carViewHandle;

        public CarView CarView { get; }
        public CarModel CarModel { get; }

        private Transform _carViewTransform;
        private TrafficLightView _observingTrafficLightView;
        private CancellationTokenSource _movingCts;

        public static event Action CarCrashed; 

        public static async UniTask<CarPresenter> Initialize(AssetReferenceGameObject viewReference, CarModel model)
        {
            var handle = Addressables.InstantiateAsync(viewReference);

            var gameObject = await handle;
            var view = gameObject.GetComponent<CarView>();

            if (ReferenceEquals(view, null)) 
                throw new ArgumentException($"View type mismatch or missing. Required type: {nameof(Car.CarView)}");
            
            var presenter = new CarPresenter(handle, view, model);

            return presenter;
        }

        private CarPresenter(AsyncOperationHandle<GameObject> viewHandle, CarView view, CarModel model)
        {
            _carViewHandle = viewHandle;
            CarView = view;
            CarModel = model;

            view.SetPresenter(this);
            
            view.Speed = model.Speed;

            _carViewTransform = view.transform;
            _carViewTransform.position = model.Position;
            _carViewTransform.rotation = model.Rotation;
        }
        
        public void SetParent(Transform parent)
        {
            _carViewTransform.parent = parent;
        }

        public void StartMoving()
        {
            CarView.CanMove = true;
        }
        
        public void StopMoving(bool velocityFreeze)
        {
            CarView.CanMove = false;
            
            if(velocityFreeze) 
                CarView.SetVelocity(Vector3.zero);
        }

        public void OnViewTransformUpdate(Vector3 position, Quaternion rotation)
        {
            CarModel.Position = position;
            CarModel.Rotation = rotation;
        }
        
        public void OnViewTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CarView carView) &&
                carView.CarPresenter.CarModel.RoadIndex == CarModel.RoadIndex)
            {
                StopMoving(true);
            }
            else if(other.TryGetComponent(out TrafficLightStopZone trafficLightStopZone) && 
                    !trafficLightStopZone.TrafficLightView.IsPassing)
            {
                SubscribeToTrafficLight(trafficLightStopZone.TrafficLightView);
                StopMoving(true);
            }
        }

        public void OnViewTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out CarView carView) &&
                carView.CarPresenter.CarModel.RoadIndex == CarModel.RoadIndex)
            {
                StartMoving();
            }
            else if(other.TryGetComponent(out TrafficLightStopZone _))
            {
                UnsubscribeFromTrafficLight();
                StartMoving();
            }
        }

        public void OnViewCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out CarView carView) && 
                carView.CarPresenter.CarModel.RoadIndex != CarModel.RoadIndex)
            {
                CarCrashed?.Invoke();
            }
        }

        private void SubscribeToTrafficLight(TrafficLightView trafficLightView)
        {
            if (trafficLightView == null) return;
            
            UnsubscribeFromTrafficLight();
            
            _observingTrafficLightView = trafficLightView;
            
            _observingTrafficLightView.StateChanged += OnTrafficLightStateChanged;
        }

        private void UnsubscribeFromTrafficLight()
        {
            if (_observingTrafficLightView == null) return;
            
            _observingTrafficLightView.StateChanged -= OnTrafficLightStateChanged;
            
            _observingTrafficLightView = null;
        }

        private void OnTrafficLightStateChanged(bool isPassing)
        {
            if (isPassing)
            {
                StartMoving();
            }
            else
            {
                StopMoving(true);
            }
        }
        
        public void Dispose()
        {
            UnsubscribeFromTrafficLight();
            
            Addressables.Release(_carViewHandle);
        }
    }
}
