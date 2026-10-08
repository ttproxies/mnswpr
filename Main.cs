using System;

namespace mnswpr
{
    class Game
    {
        public static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine("Usage: dotnet run <w> <h> <mines>");
                return 1;
            }

            const string allowedActions = "UF";
            Console.WriteLine("refactor!");

            int w = int.Parse(args[0]);
            int h = int.Parse(args[1]);
            int mines = int.Parse(args[2]);

            var board = new Board(w, h, mines);
            bool playing = false;

            char actType = '.';
            int actCol = -1, actRow = -1;

            // Game init loop
            while (!playing)
            {
                board.DisplayBoard();

                (actType, actCol, actRow) = GetAction(board.w, board.h, allowedActions);
                if (actType == 'U')
                {
                    board.GenerateMines(actRow, actCol);
                    board.RemoveStarterFlags();
                    playing = true;
                }

                HandleAction(actType, actRow, actCol, board);
            }

            // Main game loop
            while (playing)
            {
                board.DisplayBoard();

                (actType, actCol, actRow) = GetAction(board.w, board.h, allowedActions);

                playing = HandleAction(actType, actRow, actCol, board);
                // playing = board.AllMinesFlagged();
            }

            return 0;
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

        // Prompts the player to input an action
        private static (char, int, int) GetAction(int boardWidth, int boardHeight, string allowedActions)
        {
            Console.Write("\nAction:\n> ");
            string? inputAction = Console.ReadLine();

            while (string.IsNullOrEmpty(inputAction) || !IsActionValid(inputAction, boardWidth, boardHeight, allowedActions))
            {
                Console.Write("\nInvalid input, please try again:\n> ");
                inputAction = Console.ReadLine();
            }

            string[] inputParams = inputAction.Split();

            return (Convert.ToChar(inputParams[0]), Convert.ToInt32(inputParams[1]), Convert.ToInt32(inputParams[2]));
        }

        // Do the dirty work of validating user input
        private static bool IsActionValid(string actionStr, int boardWidth, int boardHeight, string allowedActions)
        {
            string[] actionParams = actionStr.Trim().Split();
            if (actionStr.Split().Length != 3)
            {
                return false;
            }
            if (actionParams[0].Length != 1)
            {
                return false;
            }

            char actionType = Convert.ToChar(actionParams[0]);
            int[] actionCoords;
            try
            {
                actionCoords = [Convert.ToInt32(actionParams[1]), Convert.ToInt32(actionParams[2])];
            }
            catch (Exception)
            {
                return false;
            }

            if (!allowedActions.Contains(actionType))
            {
                return false;
            }

            if (actionCoords[0] < 0 || actionCoords[0] >= boardWidth)
            {
                return false;
            }

            if (actionCoords[1] < 0 || actionCoords[1] >= boardHeight)
            {
                return false;
            }

            return true;
        }
    }
}