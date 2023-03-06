using UnityEngine;

namespace TrafficGame.Scripts.Level.TrafficLight
{
    public class TrafficLightStopZone : MonoBehaviour
    {
        public TrafficLightView TrafficLightView { get; private set; }

        public void Init(TrafficLightView trafficLightView)
        {
            TrafficLightView = trafficLightView;
        }
    }
}