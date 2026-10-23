namespace mnswpr
{
    class Game
    {
        public static int Main(string[] args)
        {
            int gameBoardWidth = int.Parse(args[0]);
            int gameBoardHeight = int.Parse(args[1]);
            int gameBoardMines = int.Parse(args[2]);

            var gameManager = new GameManager(gameBoardWidth, gameBoardHeight, gameBoardMines);

            while (gameManager.Playing)
            {
                gameManager.DisplayBoard();
                
                switch (gameManager.State)
                {
                    case GameManager.GameState.Idling:
                        gameManager.GetAction();
                        gameManager.HandleAction();
                        break;

                    case GameManager.GameState.Playing:
                        gameManager.GetAction();
                        gameManager.HandleAction();
                        break;

                    default:
                        break;
                }
            }

            return 0;
        }

        private bool IsValidGameArgs(string[] args)
        {
            return true;
        }
    }
}