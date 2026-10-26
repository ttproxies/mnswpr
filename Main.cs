namespace mnswpr
{
    class Game
    {
        public static int Main(string[] args)
        {
            if (!IsValidGameArgs(args)) return 1;

            int gameBoardWidth = int.Parse(args[0]);
            int gameBoardHeight = int.Parse(args[1]);
            int gameBoardMines = int.Parse(args[2]);

            var gameManager = new GameManager(gameBoardWidth, gameBoardHeight, gameBoardMines);

            while (gameManager.Playing)
            {
                gameManager.DisplayBoard();
                gameManager.GameStatusCheck();
                                
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

        private static bool IsValidGameArgs(string[] args)
        {
            return true;
        }
    }
}