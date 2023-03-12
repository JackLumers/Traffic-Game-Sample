using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficGame.Scripts.Reusable
{
    [RequireComponent(typeof(Collider))]
    public class EventTriggerVolume : MonoBehaviour
    {
        private readonly HashSet<GameObject> _objectsInVolume = new();

        public event Action<GameObject> TriggerEnter;
        public event Action<GameObject> TriggerExit;
        
        public IReadOnlyCollection<GameObject> ObjectsInVolume => _objectsInVolume;

        private void OnTriggerEnter(Collider other)
        {
            var otherGameObject = other.gameObject;
            
            _objectsInVolume.Add(otherGameObject);
            TriggerEnter?.Invoke(otherGameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            var otherGameObject = other.gameObject;
            
            _objectsInVolume.Remove(otherGameObject);
            TriggerExit?.Invoke(otherGameObject);
        }

        private void OnDestroy()
        {
            TriggerEnter = null;
            TriggerExit = null;
        }
    }
}