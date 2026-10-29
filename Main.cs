namespace mnswpr
{
    class Game
    {
        public static int Main(string[] args)
        {   
            Welcome();

            // Init game manager
            var gameManager = new GameManager();

            while (gameManager.Playing)
            {
                if (gameManager.State == GameManager.GameState.Completed
                 || gameManager.State == GameManager.GameState.Exploded) gameManager.Playing = false;

                gameManager.DisplayBoard();
                                
                switch (gameManager.State)
                {
                    case GameManager.GameState.Exploded or GameManager.GameState.Completed:
                        gameManager.ShowGameOver();

                        // Reinitialize a gameManager if retrying
                        Utility.GetAction("Would you like to try again? [y/n]", Utility.RETRY_PATTERN, (retryInput) =>
                        {
                            if (retryInput == "y") gameManager = new GameManager();
                            else Console.WriteLine("Thanks for playing!");
                        });

                        break;

                    default:
                        gameManager.GetGameInput();
                        gameManager.PerformUserAction();
                        break;
                }
            }

            return 0;
        }

        private static void Welcome()
        {
            Console.Clear();
            Console.WriteLine(Config.TITLE_ASCII);
            Console.WriteLine(Config.ASCII_BORDER);
        }
    }
}