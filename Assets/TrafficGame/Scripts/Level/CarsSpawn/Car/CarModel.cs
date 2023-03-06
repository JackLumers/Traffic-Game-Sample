using System;
using UnityEngine;

namespace TrafficGame.Scripts.Level.CarsSpawn.Car
{
    [Serializable]
    public class CarModel
    {
        [SerializeField] private Vector3 _position;
        [SerializeField] private Quaternion _rotation;
        [SerializeField] private int _roadIndex;
        [SerializeField] private float _speed;

        public event Action<float> SpeedChanged;
        
        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        public Quaternion Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public int RoadIndex
        {
            get => _roadIndex;
            set => _roadIndex = value;
        }

        public float Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                SpeedChanged?.Invoke(_speed);
            }
        }

        public CarModel(Vector3 position, Quaternion rotation, int roadIndex, float speed)
        {
            Position = position;
            Rotation = rotation;
            RoadIndex = roadIndex;
            Speed = speed;
        }
    }
}
