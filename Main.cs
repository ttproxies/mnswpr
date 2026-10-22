using System;
using System.ComponentModel.DataAnnotations;

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
                
            }

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

        private bool IsValidGameArgs(string[] args)
        {
            return true;
        }
    }
}