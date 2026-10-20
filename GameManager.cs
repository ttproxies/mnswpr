namespace mnswpr
{
    class GameManager(int boardWidth, int boardHeight, int mines)
    {
        private bool playing = false;
        private int actionX, actionY;
        private Utility.PlayerAction actionType;

        private readonly int boardWidth = boardWidth;
        private readonly int boardHeight = boardHeight;
        private readonly int mines = mines;
        private readonly Board board = new(boardWidth, boardHeight, mines);

        public void GameInit()
        {
        
        }

        private static (Utility.PlayerAction action, (int x, int y)) GetAction(int boardWidth, int boardHeight)
        {
            Console.Write("\nAction:\n> ");
            string? inputAction = Console.ReadLine();

            while (string.IsNullOrEmpty(inputAction) || !Utility.IsActionValid(inputAction, boardWidth, boardHeight))
            {
                Console.Write("\nInvalid input, please try again:\n> ");
                inputAction = Console.ReadLine();
            }

            string[] inputParams = inputAction.Split();

            return (Utility.actionCharToEnum[Convert.ToChar(inputParams[0])], (Convert.ToInt32(inputParams[1]), Convert.ToInt32(inputParams[2])));
        }


        // Handles action side-effects and returns whether game can continue
        private static bool HandleAction(int actType, int actRow, int actCol, Board board)
        {
            switch (actType)
            {
                case 'U':
                    if (board[action])
                    {
                        if (board.IsMine(actRow, actCol))
                        {
                            board.ExplodeMines();
                            Console.WriteLine("Oops! You've exploded!");
                            return false;
                        }
                        else
                        {
                            board.Flood(actRow, actCol);
                        }
                    }

                    break;

                case 'F':
                    board.ToggleFlag(actRow, actCol);
                    break;
            }
            return true;
        }
    }
}
