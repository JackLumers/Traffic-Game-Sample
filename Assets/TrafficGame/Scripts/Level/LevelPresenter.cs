using System;
using Cysharp.Threading.Tasks;
using TrafficGame.Scripts.EventTriggerVolumes;
using TrafficGame.Scripts.Level.CarsSpawn.Car;
using TrafficGame.Scripts.Level.TrafficLight;
using TrafficGame.Scripts.Screens.GameOverScreen;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TrafficGame.Scripts.Level
{
    public class LevelPresenter : IDisposable
    {
        private readonly AsyncOperationHandle<GameObject> _levelViewHandle;
        private readonly LevelModel _levelModel;
        private readonly LevelView _levelView;

        private TrafficLightPresenter _trafficLightPresenter;
        
        private GameOverScreenPresenter _gameOverScreenPresenter;

        private const int MaxScore = 20;
        
        public Camera RenderingCamera => _levelView.RenderingCamera;

        public event Action RestartRequested;

        public static async UniTask<LevelPresenter> Initialize(AssetReferenceGameObject viewReference, LevelModel model)
        {
            var handle = Addressables.InstantiateAsync(viewReference);

            var gameObject = await handle;
            var view = gameObject.GetComponent<LevelView>();

            if (ReferenceEquals(view, null)) 
                throw new ArgumentException($"View type mismatch or missing. Required type: {nameof(LevelView)}");
            
            var presenter = new LevelPresenter(handle, view, model);
            await presenter.InnerInitialize();

            return presenter;
        }
        
        private LevelPresenter(AsyncOperationHandle<GameObject> viewHandle, LevelView view, LevelModel model)
        {
            _levelViewHandle = viewHandle;
            _levelView = view;
            _levelModel = model;

            view.SetPresenter(this);
            model.ScoreUpdated += OnModelScoreUpdated;
            model.Paused += Pause;
            
            CarPresenter.CarCrashed += OnCarCrashed;
            
            view.SetScore(model.Score);
        }

        private async UniTask InnerInitialize()
        {
            _trafficLightPresenter = await TrafficLightPresenter.Initialize(_levelView.TrafficLightViewReference, _levelModel.TrafficLightModel);

            _trafficLightPresenter.SetViewAnchor(_levelView.TrafficLightViewAnchor);
            
            await _levelView.SpawnCarsFromModels(_levelModel.CarModels);

            _levelView.CarSpawned += OnViewCarSpawned;
            _levelView.CarRemoved += OnViewCarRemoved;

            if (_levelModel.IsCarsCrashed)
            {
                await GameOver(false);
            }
            else if(_levelModel.Score >= 20)
            {
                await GameOver(true);
            }
            else
            {
                Pause(_levelModel.IsPaused);
            }
        }

        public void OnViewPauseClicked()
        {
            _levelModel.IsPaused = !_levelModel.IsPaused;
        }

        private void OnViewCarSpawned(CarModel carModel)
        {
            _levelModel.CarModels.Add(carModel);
        }

        private void OnViewCarRemoved(CarModel carModel)
        {
            _levelModel.CarModels.Remove(carModel);
        }

        private void Pause(bool pause)
        {
            _trafficLightPresenter.BlockViewInput(pause);

            _levelView.SetPauseVisualState(pause);
            _levelView.EnableRandomCarsSpawning(!pause);
            Time.timeScale = pause ? 0f : 1f;
        }

        private async UniTask GameOver(bool isWin)
        {
            CarPresenter.CarCrashed -= OnCarCrashed;

            if (_gameOverScreenPresenter == null)
            {
                Pause(true);

                if (isWin)
                {
                    _gameOverScreenPresenter = await GameOverScreenPresenter.Initialize(_levelView.GameOverWinViewReference,
                        new GameOverScreenModel(_levelModel.Score));
                }
                else
                {
                    _gameOverScreenPresenter = await GameOverScreenPresenter.Initialize(_levelView.GameOverDefeatViewReference,
                        new GameOverScreenModel(_levelModel.Score));
                }
            
                _gameOverScreenPresenter.RestartClicked += OnGameOverScreenRestartClicked;
            }
        }

        private void OnGameOverScreenRestartClicked()
        {
            RestartRequested?.Invoke();
        }

        private void OnModelScoreUpdated(int newScore)
        {
            _levelView.SetScore(newScore);
            
            if (newScore >= MaxScore)
            {
                GameOver(true).Forget();
            }
        }
        
        public void OnCarTouchesFinishPoint(CarFinishVolume finishVolume, CarPresenter carPresenter)
        {
            _levelView.RemoveCar(carPresenter);
            
            if(finishVolume.IsScoring) 
                _levelModel.Score++;
        }

        private void OnCarCrashed()
        {
            _levelModel.IsCarsCrashed = true;
            
            GameOver(false).Forget();
        }

        public void Dispose()
        {
            _trafficLightPresenter?.Dispose();
            _trafficLightPresenter = null;
            
            _gameOverScreenPresenter?.Dispose();
            _gameOverScreenPresenter = null;
            
            _levelView.CarSpawned -= OnViewCarSpawned;
            _levelView.CarSpawned -= OnViewCarRemoved;
            
            _levelModel.ScoreUpdated -= OnModelScoreUpdated;
            _levelModel.Paused -= Pause;
            
            CarPresenter.CarCrashed -= OnCarCrashed;

            RestartRequested = null;
            
            Addressables.Release(_levelViewHandle);
        }
    }
}
