using System;
using System.CodeDom.Compiler;

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
            board.ChangeState(1, 2, Board.TileState.Flagged); // Flag tile
            board.ChangeState(7, 5, Board.TileState.Uncovered); // Uncover tile
            board.DisplayBoard();
            
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

        public readonly int w, h;
        private TileState[] boardStates;
        private TileState[] boardMines;

        public Board(int w, int h, int mines)
        {
            this.w = w;
            this.h = h;
            this.boardStates = new TileState[w*h];
            this.boardMines = new TileState[w*h];

            Array.Fill(boardStates, TileState.Unknown);
            Array.Fill(boardMines, TileState.Unknown);
        }

        // Distibutes mines across the board 
        private void GenerateMines(int clickRow, int clickColumn)
        {
            
        }

        private int MinesInProximity(int row, int column)
        {
            return 2;
        }

        // Displays the current state of the board
        public void DisplayBoard()
        {
            int neighboringMines;
            for (int i = 0; i < this.h; i++)
            {
                for (int j = 0; j < this.w; j++)
                {
                    
                    switch (this.boardStates[i*w + j])
                    {
                        case TileState.Unknown:
                            Console.Write("◩ ");
                            break;

                        case TileState.Uncovered:
                            neighboringMines = MinesInProximity(i, j);
                            Console.Write(neighboringMines > 0 ? neighboringMines : "⚬ "); // Mine proximity logic to be added
                            break;

                        case TileState.Flagged:
                            Console.Write("⚐ ");
                            break;

                        case TileState.Exploded:
                            Console.Write("✴︎ ");
                            break;
                    }
                }
                Console.WriteLine();
            }
        }

        // Changes the tile's state to the specified state code
        public void ChangeState(int row, int column, TileState state)
        {
            this.boardStates[this.w * row + column] = state;
        }    
    }
}