using UnityEngine;
using UnityEngine.UI;

namespace TrafficGame.Scripts.Screens.GameOverScreen.Views
{
    public class GameOverScreenWinView : MonoBehaviour, IGameOverScreenView
    {
        [SerializeField] private Text _scoreText;
        [SerializeField] private Button _restartButton;

        public GameOverScreenPresenter Presenter { get; private set; }

        public void SetPresenter(GameOverScreenPresenter presenter)
        {
            Presenter = presenter;
            _restartButton.onClick.AddListener(presenter.OnViewRestartButtonClicked);
        }

        public void SetScore(int score)
        {
            _scoreText.text = $"You win! Score: {score}";
        }
    }
}
