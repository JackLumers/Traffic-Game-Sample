namespace TrafficGame.Scripts.Screens.GameOverScreen
{
    public interface IGameOverScreenView
    {
        public void SetPresenter(GameOverScreenPresenter presenter);
        
        public void SetScore(int score);
    }
}