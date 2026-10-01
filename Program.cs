using System;

namespace Game
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

            int w = int.Parse(args[0]);
            int h = int.Parse(args[1]);
            int mines =  int.Parse(args[2]); 

            var board = new Board(w, h, mines);
            board.DisplayBoard();
            board.ChangeState(1, 2, Board.TileState.Flagged); // Flag tile
            board.ChangeState(1, 2, Board.TileState.Uncovered); // Uncover tile
            
            return 0;
        }
    }

    class Board
    {
        public enum TileState
        {
            Unknown,
            Uncovered,
            Flagged,
            Exploded
        }

        const char FLAG = '⚐';
        const char TILE = '◩';
        public readonly int w, h;
        private int[] boardStates;
        private int[] boardMines;

        public Board(int w, int h, int mines)
        {
            this.w = w;
            this.h = h;
            this.boardStates = new int[w*h];
            this.boardMines = new int[w*h];

            Array.Fill(boardStates, 0);
            Array.Fill(boardMines, 0);
        }

        public void DisplayBoard()
        {
            for (int i = 0; i < this.h; i++)
            {
                for (int j = 0; j < this.w; j++)
                {
                    Console.Write(TILE + " ");
                }
                Console.WriteLine();
            }
        }

        // Changes the tile's state to the specified state code
        public void ChangeState(int row, int column, TileState state)
        {
            this.boardStates[this.w * row + column] = (int) state;
        }    
    }
}