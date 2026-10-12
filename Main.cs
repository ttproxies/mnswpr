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
            
            var game = new GameManager();

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
        
    }
}