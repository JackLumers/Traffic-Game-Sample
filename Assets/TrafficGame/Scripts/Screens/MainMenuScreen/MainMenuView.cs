using UnityEngine;
using UnityEngine.UI;

namespace TrafficGame.Scripts.Screens.MainMenuScreen
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Text _startButtonText;
        
        public void Initialize(MainMenuPresenter mainMenuPresenter)
        {
            _startButton.onClick.AddListener(mainMenuPresenter.OnViewStartButtonClicked);
        }

        public void SetStartButtonState(bool isGameSaveExist)
        {
            _startButtonText.text = isGameSaveExist ? "Continue" : "Start";
        }
    }
}