namespace TrafficGame.Scripts.Screens.MainMenuScreen
{
    public class MainMenuModel
    {
        public bool GameSaveExist { get; set; }
        
        public MainMenuModel(bool gameSaveExist)
        {
            GameSaveExist = gameSaveExist;
        }
    }
}