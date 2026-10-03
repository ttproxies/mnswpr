using System;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

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
            int mines = int.Parse(args[2]);

            var board = new Board(w, h, mines);
            bool playing = true;

            // Main game loop
            while (playing)
            {
                Console.Write("Action:\n> ");
                string inputAction;
                do
                {
                    inputAction = Console.ReadLine();
                }
                while (!IsActionValid(inputAction));

                string[] actionArgs = inputAction.Split();
                char actType = Convert.ToChar(actionArgs[0]);
                int actRow = Convert.ToInt32(actionArgs[1]);
                int actCol = Convert.ToInt32(actionArgs[2]);
                int actInd = actRow * board.w + actCol;

                switch (actType)
                {
                    case 'U':
                        if (board.boardStates[actInd] == Board.TileState.Unknown)
                        {
                            if (board.boardMines[actInd])
                        }

                        break;

                    case 'F':
                        board.ToggleFlag(actRow, actCol);
                        break;
                }
            }

            return 0;
        }

        private static bool IsActionValid(string actionStr)
        {
            return true;
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

        public readonly int w, h, mines;
        public TileState[] boardStates;
        private bool[] boardMines;

        public Board(int w, int h, int mines)
        {
            this.w = w;
            this.h = h;
            this.mines = mines;
            this.boardStates = new TileState[w * h];
            this.boardMines = new bool[w * h];

            Array.Fill(boardStates, TileState.Unknown);
            Array.Fill(boardMines, false);
        }

        // Distibutes mines across the board and sets boardMines accordingly
        private void GenerateMines(int clickRow, int clickColumn)
        {
            Random rand = new();
            int[] pickedIndices = new int[this.mines - 1];
            int curIndex;
            for (int i = 0; i < this.mines; i++)
            {
                do
                {
                    curIndex = rand.Next(this.w * this.h);
                }
                while (!pickedIndices.Contains(curIndex));
                this.boardMines[curIndex] = true;
            }
        }

        // Returns number of neighboring mines
        private int MinesInProximity(int row, int column)
        {
            int mines = 0;
            int curInd;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    curInd = row + this.w * i + column + j;
                    if (curInd >= 0 && this.boardMines[curInd])
                    {
                        mines++;
                    }
                }
            }

            return mines;
        }

        // FLood tile uncovery
        private void Flood(int row, int col, List<int> visited, Queue<int> queue)
        {
            int ind = this.w * row + col;
            visited.Add(ind);

            if (this.boardStates[ind] != TileState.Flagged)
            {
                return;
            }

            ChangeState(row, col, TileState.Uncovered);

            // Edge case for root vertex
            if (queue.Count == 0)
            {
                queue.Enqueue(ind);
                Flood(row, col, visited, queue);
            }

            if (MinesInProximity(row, col) == 0)
            {
                int adjInd;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        adjInd = ind + this.w * i + j;
                        if (!visited.Contains(adjInd))
                        {
                            queue.Enqueue(adjInd);
                        }
                    }
                }
            }


        }

        // Displays the current state of the board
        public void DisplayBoard()
        {
            int neighboringMines;
            for (int i = 0; i < this.h; i++)
            {
                for (int j = 0; j < this.w; j++)
                {

                    switch (this.boardStates[i * w + j])
                    {
                        case TileState.Unknown:
                            Console.Write("◩ ");
                            break;

                        case TileState.Uncovered:
                            neighboringMines = MinesInProximity(i, j);
                            Console.Write(neighboringMines > 0 ? neighboringMines + " " : "⚬ "); // Mine proximity logic to be added
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

        public TileState GetTileState(int row, int column)
        {
            return this.boardStates[this.w * row + column];
        }

        public void ToggleFlag(int row, int column)
        {
            // Maybe try to make this more elegant...
            if (GetTileState(row, column) == TileState.Unknown)
            {
                ChangeState(row, column, TileState.Flagged);
            }

            if (GetTileState(row, column) == TileState.Flagged)
            {
                ChangeState(row, column, TileState.Unknown);
            }
        }
    }
}