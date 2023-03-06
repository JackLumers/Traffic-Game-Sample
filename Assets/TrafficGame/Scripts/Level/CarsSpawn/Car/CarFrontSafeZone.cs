using UnityEngine;

namespace TrafficGame.Scripts.Level.CarsSpawn.Car
{
    public class CarFrontSafeZone : MonoBehaviour
    {
        public CarView CarView { get; private set; }

        public void Init(CarView carView)
        {
            CarView = carView;
        }
    }
}