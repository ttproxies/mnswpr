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

            int gameBoardWidth = int.Parse(args[0]);
            int gameBoardHeight = int.Parse(args[1]);
            int gameBoardMines = int.Parse(args[2]);

            var gameManager = new GameManager(gameBoardWidth, gameBoardHeight, gameBoardMines);


            var board = new Board(w, h, mines);
            bool playing = false;


            // char actType = '.';
            // int actCol = -1, actRow = -1;

            // // Game init loop
            // while (!playing)
            // {
            //     board.DisplayBoard();

            //     (actType, actCol, actRow) = GetAction(board.w, board.h, allowedActions);
            //     if (actType == 'U')
            //     {
            //         board.GenerateMines(actRow, actCol);
            //         board.RemoveStarterFlags();
            //         playing = true;
            //     }

            //     HandleAction(actType, actRow, actCol, board);
            // }

            // // Main game loop
            // while (playing)
            // {
            //     board.DisplayBoard();

            //     (actType, actCol, actRow) = GetAction(board.w, board.h, allowedActions);

            //     playing = HandleAction(actType, actRow, actCol, board);
            //     // playing = board.AllMinesFlagged();
            // }

            return 0;
        }
    }
}