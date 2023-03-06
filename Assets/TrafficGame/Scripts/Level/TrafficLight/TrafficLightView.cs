using System;
using TrafficGame.Scripts.Input;
using UnityEngine;

namespace TrafficGame.Scripts.Level.TrafficLight
{
    public class TrafficLightView : MonoBehaviour, IPointerHandler
    {
        [SerializeField] private MeshRenderer _lightRenderer;
        [SerializeField] private Material _redMaterial;
        [SerializeField] private Material _greenMaterial;
        [SerializeField] private TrafficLightStopZone _trafficLightStopZone;

        public TrafficLightPresenter TrafficLightPresenter { get; private set; }

        public GameObject GameObject => gameObject;
        
        public bool IsPassing { get; private set; }
        
        public bool InputBlocked { get; set; }

        public event Action<bool> StateChanged;

        public void SetPresenter(TrafficLightPresenter trafficLightPresenter)
        {
            TrafficLightPresenter = trafficLightPresenter;
            _trafficLightStopZone.Init(this);
        }
        
        public void ChangeState(bool isPassing)
        {
            _lightRenderer.material = isPassing ? _greenMaterial : _redMaterial;
            IsPassing = isPassing;
            StateChanged?.Invoke(isPassing);
        }

        public void OnClicked()
        {
            if(!InputBlocked)
                TrafficLightPresenter.OnViewClicked();
        }

        private void OnDestroy()
        {
            StateChanged = null;
        }
    }
}
