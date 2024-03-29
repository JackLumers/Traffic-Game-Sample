﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TrafficGame.Scripts.EventTriggerVolumes;
using TrafficGame.Scripts.Level.CarsSpawn;
using TrafficGame.Scripts.Level.CarsSpawn.Car;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace TrafficGame.Scripts.Level
{
    public class LevelView : MonoBehaviour
    {
        [SerializeField] private Camera _renderingCamera;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Text _pauseButtonText;
        [SerializeField] private Text _scoreText;
        [SerializeField] private CarsSpawnController _carsSpawnController;
        [SerializeField] private List<CarFinishVolume> _carsFinishPoints;
        [SerializeField] private Transform _trafficLightViewAnchor;

        [SerializeField] private AssetReferenceGameObject _trafficLightViewReference;
        [SerializeField] private AssetReferenceGameObject _gameOverWinViewReference;
        [SerializeField] private AssetReferenceGameObject _gameOverDefeatViewReference;
        
        public Camera RenderingCamera => _renderingCamera;

        public Transform TrafficLightViewAnchor => _trafficLightViewAnchor;
        
        public AssetReferenceGameObject TrafficLightViewReference => _trafficLightViewReference;
        public AssetReferenceGameObject GameOverWinViewReference => _gameOverWinViewReference;
        public AssetReferenceGameObject GameOverDefeatViewReference => _gameOverDefeatViewReference;
        
        public LevelPresenter LevelPresenter { get; private set; }

        public event Action<CarModel> CarSpawned;
        public event Action<CarModel> CarRemoved;

        public void SetPresenter(LevelPresenter levelPresenter)
        {
            LevelPresenter = levelPresenter;
            _pauseButton.onClick.AddListener(LevelPresenter.OnViewPauseClicked);

            _carsSpawnController.CarSpawned += OnCarSpawned;
            _carsSpawnController.CarRemoved += OnCarRemoved;
            
            foreach (var carsFinishPoint in _carsFinishPoints)
            {
                carsFinishPoint.TriggerEnter += OnFinishPointTrigger;
            }
        }
        
        private void OnCarSpawned(CarModel carModel)
        {
            CarSpawned?.Invoke(carModel);
        }
        
        private void OnCarRemoved(CarModel carModel)
        {
            CarRemoved?.Invoke(carModel);
        }

        private void OnFinishPointTrigger(CarFinishVolume carFinishVolume, GameObject triggerObject)
        {
            if (triggerObject.TryGetComponent(out CarView carView))
            {
                LevelPresenter.OnCarTouchesFinishPoint(carFinishVolume, carView.CarPresenter);
            }
        }

        public async UniTask SpawnCarsFromModels(IEnumerable<CarModel> carModels)
        {
            await _carsSpawnController.SpawnFromModels(carModels, gameObject.GetCancellationTokenOnDestroy());
        }

        public void RemoveCar(CarPresenter carPresenter)
        {
            _carsSpawnController.RemoveCar(carPresenter);
        }

        public void EnableRandomCarsSpawning(bool enable)
        {
            _carsSpawnController.EnableRandomSpawning(enable);
        }

        public void SetScore(int score)
        {
            _scoreText.text = $"Score: {score}";
        }

        public void SetPauseVisualState(bool isPaused)
        {
            _pauseButtonText.text = isPaused ? ">" : "||";
        }
        
        private void OnDestroy()
        {
            CarSpawned = null;
            CarRemoved = null;
            
            foreach (var carsFinishPoint in _carsFinishPoints)
            {
                carsFinishPoint.TriggerEnter -= OnFinishPointTrigger;
            }
        }
    }
}