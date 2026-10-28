namespace mnswpr
{
    class Game
    {
        public static int Main(string[] args)
        {   
            // Game title
            Console.WriteLine(Config.TITLE_ASCII);
            Console.WriteLine(Config.ASCII_BORDER);

            var gameManager = new GameManager();

            while (gameManager.Playing)
            {
                if (gameManager.State == GameManager.GameState.Completed
                 || gameManager.State == GameManager.GameState.Exploded) gameManager.Playing = false;

                gameManager.DisplayBoard();
                                
                switch (gameManager.State)
                {
                    default:
                        gameManager.GetGameInput();
                        gameManager.PerformUserAction();
                        break;
                }
            }

            return 0;
        }
    }
}