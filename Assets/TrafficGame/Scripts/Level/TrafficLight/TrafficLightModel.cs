using System;
using UnityEngine;

namespace TrafficGame.Scripts.Level.TrafficLight
{
    [Serializable]
    public class TrafficLightModel
    {
        [SerializeField] private bool _isPassing;
        
        public event Action<bool> StateChanged;

        public bool IsPassing
        {
            get => _isPassing;
            set
            {
                _isPassing = value;
                StateChanged?.Invoke(_isPassing);
            }
        }

        public TrafficLightModel(bool isPassing)
        {
            _isPassing = isPassing;
        }
    }
}
