using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TrafficGame.Scripts.Screens.MainMenuScreen
{
    public class MainMenuPresenter : IDisposable
    {
        private AsyncOperationHandle<GameObject> _mainMenuViewHandle;

        private MainMenuView _mainMenuView;
        private MainMenuModel _mainMenuModel;
        
        public Action StartButtonClicked;
        
        public static async UniTask<MainMenuPresenter> Initialize(AssetReferenceGameObject mainMenuViewReference, MainMenuModel model)
        {
            var handle = Addressables.InstantiateAsync(mainMenuViewReference);
            
            var mainMenuGameObject = await handle;
            var mainMenuView = mainMenuGameObject.GetComponent<MainMenuView>();

            if (ReferenceEquals(mainMenuView, null)) 
                throw new ArgumentException($"View type mismatch or missing. Required type: {nameof(MainMenuView)}");
            
            return new MainMenuPresenter(handle, mainMenuView, model);
        }
        
        private MainMenuPresenter(AsyncOperationHandle<GameObject> viewHandle, MainMenuView view, MainMenuModel model)
        {
            _mainMenuViewHandle = viewHandle;
            
            _mainMenuView = view;
            _mainMenuModel = model;
            
            _mainMenuView.Initialize(this);

            _mainMenuView.SetStartButtonState(model.GameSaveExist);
        }
        
        public void OnViewStartButtonClicked()
        {
            StartButtonClicked?.Invoke();
        }

        public void Dispose()
        {
            Addressables.Release(_mainMenuViewHandle);
        }
    }
}