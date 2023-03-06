using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using TrafficGame.Scripts.Input;
using TrafficGame.Scripts.Level;
using TrafficGame.Scripts.Screens.MainMenuScreen;

namespace TrafficGame.Scripts
{
    public class GameInstaller : MonoBehaviour
    {
        [SerializeField] private LevelModel _levelModel;
        [SerializeField] private AssetReferenceGameObject _mainMenuViewReference;
        [SerializeField] private AssetReferenceGameObject _levelViewReference;

        private InputController _inputController;

        private MainMenuPresenter _mainMenuPresenter;
        private LevelPresenter _levelPresenter;
        
        private void Awake()
        {
            _levelModel.LoadFromFile();
            LoadMainMenu().Forget();
        }

        private async UniTaskVoid LoadMainMenu()
        {
            _mainMenuPresenter = await MainMenuPresenter.Initialize(_mainMenuViewReference, new MainMenuModel(_levelModel.IsSaveExist()));
            
            _mainMenuPresenter.StartButtonClicked += OnStartButtonClicked;
        }

        private void OnStartButtonClicked()
        {
            _mainMenuPresenter.StartButtonClicked -= OnStartButtonClicked;
            LoadLevel().Forget();
        }

        private async UniTask LoadLevel()
        {
            _levelPresenter?.Dispose();
            _inputController?.Dispose();
            
            _levelPresenter = await LevelPresenter.Initialize(_levelViewReference, _levelModel);
            _levelPresenter.RestartRequested += OnLevelPresenterRestartRequested;

            _inputController = new InputController(_levelPresenter.RenderingCamera);
            _inputController.Enable();

            _mainMenuPresenter?.Dispose();
            _mainMenuPresenter = null;
        }

        private void OnLevelPresenterRestartRequested()
        {
            _levelModel.ResetToDefaultValues();
            
            _levelModel.DeleteSave();
            _levelModel.LoadFromFile();

            LoadLevel().Forget();
        }

        private void OnDestroy()
        {
            _levelModel.SaveToFile();
            
            _levelPresenter?.Dispose();
        }
    }
}
