using System;
using System.Collections.Generic;
using TrafficGame.Scripts.Level.CarsSpawn.Car;
using TrafficGame.Scripts.Level.TrafficLight;
using TrafficGame.Scripts.SaveSystem;
using UnityEngine;

namespace TrafficGame.Scripts.Level
{
    [CreateAssetMenu(fileName = "LevelModel", menuName = "Create Level Model")]
    public class LevelModel : JsonScriptableObject
    {
        [SerializeField] private bool _carsCrashed = false;
        [SerializeField] private int _score = 0;
        [SerializeField] private bool _isPaused = false;
        [SerializeField] private List<CarModel> _carModels = new();
        [SerializeField] private TrafficLightModel _trafficLightModel = new(false);

        public event Action<int> ScoreUpdated;
        
        public event Action<bool> CarsCrashed;

        public event Action<bool> Paused; 
        
        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                ScoreUpdated?.Invoke(value);
            }
        }
        
        public bool IsCarsCrashed
        {
            get => _carsCrashed;
            set
            {
                _carsCrashed = value;
                CarsCrashed?.Invoke(value);
            }
        }
        
        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                _isPaused = value;
                Paused?.Invoke(value);
            }
        }

        public List<CarModel> CarModels => _carModels;
        public TrafficLightModel TrafficLightModel => _trafficLightModel;

        public override void ResetToDefaultValues()
        {
            _carsCrashed = false;
            _score = 0;
            _isPaused = false;
            _carModels = new List<CarModel>();
            _trafficLightModel = new TrafficLightModel(false);
        }
    }
}
