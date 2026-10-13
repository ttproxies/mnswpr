namespace mnswpr
{
    class GameManager
    {
        private bool playing = false;
        private int actionX, actionY;
        private Utility.PlayerAction actionType;

        private readonly int boardWidth;
        private readonly int boardHeight;
        private readonly int mines;

        public GameManager(int boardWidth, int boardHeight, int mines)
        {
            this.boardWidth = boardWidth;
            this.boardHeight = boardHeight;
            this.mines = mines;
        }

        public void GameInit()
        {
            
        }

        private static (Utility.PlayerAction, int, int) GetAction(int boardWidth, int boardHeight)
        {
            Console.Write("\nAction:\n> ");
            string? inputAction = Console.ReadLine();

            while (string.IsNullOrEmpty(inputAction) || !Utility.IsActionValid(inputAction, boardWidth, boardHeight))
            {
                Console.Write("\nInvalid input, please try again:\n> ");
                inputAction = Console.ReadLine();
            }

            string[] inputParams = inputAction.Split();

            return (Utility.actionCharToEnum[Convert.ToChar(inputParams[0])], Convert.ToInt32(inputParams[1]), Convert.ToInt32(inputParams[2]));
        }


        // Handles action side-effects and returns whether game can continue
        private static bool HandleAction(int actType, int actRow, int actCol, Board board)
        {
            switch (actType)
            {
                case 'U':
                    if (board.GetTileState(actRow, actCol) == Board.TileState.Unknown)
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
