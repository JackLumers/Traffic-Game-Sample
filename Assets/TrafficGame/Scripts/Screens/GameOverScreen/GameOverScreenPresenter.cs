using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TrafficGame.Scripts.Screens.GameOverScreen
{
    public class GameOverScreenPresenter : IDisposable
    {
        private AsyncOperationHandle<GameObject> _viewHandle;
        
        private readonly GameOverScreenModel _model;
        private readonly IGameOverScreenView _view;

        public event Action RestartClicked;

        public static async UniTask<GameOverScreenPresenter> Initialize(AssetReferenceGameObject viewReference, GameOverScreenModel screenModel)
        {
            var handle = Addressables.InstantiateAsync(viewReference);
        
            var gameObject = await handle;
            var view = gameObject.GetComponent<IGameOverScreenView>();
        
            if (ReferenceEquals(view, null)) 
                throw new ArgumentException($"View type mismatch or missing. Required type: {nameof(IGameOverScreenView)}");
            
            var presenter = new GameOverScreenPresenter(handle, view, screenModel);
        
            return presenter;
        }
        
        private GameOverScreenPresenter(AsyncOperationHandle<GameObject> viewHandle, IGameOverScreenView view, GameOverScreenModel model)
        {
            _viewHandle = viewHandle;
            _view = view;
            _model = model;
            
            _view.SetPresenter(this);
            _view.SetScore(model.Score);
        }

        public void OnViewRestartButtonClicked()
        {
            RestartClicked?.Invoke();   
        }
        
        public void Dispose()
        {
            RestartClicked = null;
            Addressables.Release(_viewHandle);
        }
    }
}
