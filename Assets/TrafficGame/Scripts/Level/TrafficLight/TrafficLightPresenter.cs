using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TrafficGame.Scripts.Level.TrafficLight
{
    public class TrafficLightPresenter : IDisposable
    {
        private readonly AsyncOperationHandle<GameObject> _trafficLightViewHandle;
        private readonly Transform _trafficLightTransform;

        public TrafficLightView TrafficLightView { get; }
        public TrafficLightModel TrafficLightModel { get; }

        public static async UniTask<TrafficLightPresenter> Initialize(AssetReferenceGameObject viewReference, TrafficLightModel model)
        {
            var handle = Addressables.InstantiateAsync(viewReference);

            var gameObject = await handle;
            var view = gameObject.GetComponent<TrafficLightView>();

            if (ReferenceEquals(view, null)) 
                throw new ArgumentException($"View type mismatch or missing. Required type: {nameof(TrafficLight.TrafficLightView)}");
            
            return new TrafficLightPresenter(handle, view, model);
        }
        
        private TrafficLightPresenter(AsyncOperationHandle<GameObject> viewHandle, TrafficLightView view, TrafficLightModel model)
        {
            _trafficLightViewHandle = viewHandle;
            _trafficLightTransform = view.transform;
            
            TrafficLightView = view;
            TrafficLightModel = model;
            
            TrafficLightView.SetPresenter(this);
            
            TrafficLightView.ChangeState(TrafficLightModel.IsPassing);
            TrafficLightModel.StateChanged += OnModelLightStateChanged;
        }

        public void BlockViewInput(bool block)
        {
            TrafficLightView.InputBlocked = block;
        }
        
        public void OnViewClicked()
        {
            TrafficLightModel.IsPassing = !TrafficLightModel.IsPassing;
        }

        private void OnModelLightStateChanged(bool state)
        {
            TrafficLightView.ChangeState(state);
        }
        
        public void SetViewAnchor(Transform anchor)
        {
            _trafficLightTransform.SetParent(anchor, false);
        }

        public void Dispose()
        {
            TrafficLightModel.StateChanged -= OnModelLightStateChanged;
            Addressables.Release(_trafficLightViewHandle);
        }
    }
}
