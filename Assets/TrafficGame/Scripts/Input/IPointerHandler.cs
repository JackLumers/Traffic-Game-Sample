using UnityEngine;

namespace TrafficGame.Scripts.Input
{
    public interface IPointerHandler
    {
        public void OnClicked();
        
        public GameObject GameObject { get; }
    }
}