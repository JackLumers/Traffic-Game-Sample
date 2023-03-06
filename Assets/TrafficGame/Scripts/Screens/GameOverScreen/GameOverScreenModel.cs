namespace TrafficGame.Scripts.Screens.GameOverScreen
{
    public class GameOverScreenModel
    {
        private int _score;

        public int Score => _score;

        public GameOverScreenModel(int score)
        {
            _score = score;
        }
    }
}
