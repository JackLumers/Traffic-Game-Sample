using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficGame.Scripts.EventTriggerVolumes
{
    [RequireComponent(typeof(Collider))]
    public class BaseEventTriggerVolume<T> : MonoBehaviour where T: BaseEventTriggerVolume<T>
    {
        private readonly HashSet<GameObject> _objectsInVolume = new();
        
        public event Action<T, GameObject> TriggerEnter;
        public event Action<T, GameObject> TriggerExit;
        
        public IReadOnlyCollection<GameObject> ObjectsInVolume => _objectsInVolume;

        protected virtual void OnTriggerEnter(Collider other)
        {
            var otherGameObject = other.gameObject;
            
            _objectsInVolume.Add(otherGameObject);
            TriggerEnter?.Invoke(this as T, otherGameObject);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            var otherGameObject = other.gameObject;
            
            _objectsInVolume.Remove(otherGameObject);
            TriggerExit?.Invoke(this as T, otherGameObject);
        }

        protected virtual void OnDestroy()
        {
            TriggerEnter = null;
            TriggerExit = null;
        }
    }
}