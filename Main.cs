namespace mnswpr
{
    class Game
    {
        public static int Main(string[] args)
        {   

            var preset = Config.BoardPreset.Beginner;
            var gameManager = new GameManager();

            while (gameManager.Playing)
            {
                if (gameManager.State == GameManager.GameState.Completed
                 || gameManager.State == GameManager.GameState.Exploded) gameManager.Playing = false;

                gameManager.DisplayBoard();
                                
                switch (gameManager.State)
                {
                    default:
                        gameManager.GetAction();
                        gameManager.HandleAction();
                        break;
                }
            }

            return 0;
        }
    }
}