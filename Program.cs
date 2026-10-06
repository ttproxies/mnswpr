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

            // Start round
            board.DisplayBoard();
            string inputAction = GetActionInput();


            // Main game loop
            while (playing)
            {
                board.DisplayBoard();

                inputAction = GetActionInput();

                string[] actionArgs = inputAction.Split();
                char actType = Convert.ToChar(actionArgs[0]);
                int actRow = Convert.ToInt32(actionArgs[1]);
                int actCol = Convert.ToInt32(actionArgs[2]);
                int actInd = actRow * board.w + actCol;

                switch (actType)
                {
                    case 'U':
                        if (board.GetTileState(actRow, actCol) == Board.TileState.Unknown)
                        {
                            if (board.IsMine(actRow, actCol))
                            {
                                board.ExplodeMines();
                                Console.WriteLine("Oops! You've exploded!");
                                playing = false;
                            }
                            else
                            {
                                board.Reveal(actRow, actCol);
                            }
                        }

                        break;

                    case 'F':
                        board.ToggleFlag(actRow, actCol);
                        break;

                    default:
                        break;
                }
            }

            return 0;
        }

        private static (char, int, int) GetAction()
        {
            Console.Write("\nAction:\n> ");
            string? inputAction;
            do
            {
                inputAction = Console.ReadLine();
            }
            while (!IsActionValid(inputAction));
        }

        private static bool IsActionValid(string actionStr, int boardWidth, int boardHeight)
        {
            string pattern = $"[UF] [0]";
            if (actionStr )
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
        private TileState[] boardStates;
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
            pickedIndices[0] = clickRow * this.w + clickColumn;

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
                    if (curInd >= 0 && curInd < this.w * this.h && this.boardMines[curInd])
                    {
                        mines++;
                    }
                }
            }

            Console.Write(mines);
            return mines;
        }

        // Reveals tile and performs flood filling
        public void Reveal(int row, int col)
        {
            Flood(row, col, [], []);
        }

        // Reveal all neighboring blank tiles recursively
        private void Flood(int row, int col, List<int> visited, Queue<int> queue)
        {
            // Mark current index as visited and uncover tile
            int ind = this.w * row + col;
            Console.WriteLine("Current flood tile: " + ind);
            visited.Add(ind);
            if (GetTileState(row, col) != TileState.Flagged)
            {
                SetTileState(row, col, TileState.Uncovered);
            }

            // Add neighbors to queue if not an edge tile
            if (MinesInProximity(row, col) == 0)
            {
                int adjInd;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        adjInd = ind + this.w * i + j;
                        if (!visited.Contains(adjInd) && !queue.Contains(adjInd) && IsValidAdj(adjInd, ind, i, j))
                        {
                            queue.Enqueue(adjInd);
                        }
                    }
                }
            }

            if (queue.Count == 0)
            {
                return;
            }

            int nextInd = queue.Dequeue();
            Flood((int)(nextInd / this.w), nextInd % this.w, visited, queue);
        }

        // Displays the current state of the board
        public void DisplayBoard()
        {
            Console.WriteLine("\x1b[3J");
            Console.Clear();
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

        // Returns whether an index is in range relative to original index
        private bool IsValidAdj(int adjInd, int relInd, int i, int j)
        {
            // Trivial bounds checking
            if (adjInd < 0 || adjInd >= this.w * this.h)
            {
                return false;
            }

            // Some form of wrapping must be occurring
            if (adjInd % this.w != relInd % this.w + j || (int)(adjInd / this.w) != (int)(relInd / this.w) + i)
            {
                return false;
            }

            return true;
        }

        // Sets all tiles with a mine to TileState.Exploded
        public void ExplodeMines()
        {
            for (int i = 0; i < this.h; i++)
            {
                for (int j = 0; j < this.w; j++)
                {
                    if (IsMine(i, j))
                    {
                        SetTileState(i, j, TileState.Exploded);
                    }
                }
            }
            return;
        }

        // Changes the tile's state to the specified state code
        public void SetTileState(int row, int column, TileState state)
        {
            this.boardStates[this.w * row + column] = state;
        }

        // Returns the TileState value of a board tile
        public TileState GetTileState(int row, int column)
        {
            Console.WriteLine($"{row}, {column}, {this.w * row + column}");
            return this.boardStates[this.w * row + column];
        }

        // Returns whether tile has a mine or not
        public bool IsMine(int row, int column)
        {
            return this.boardMines[this.w * row + column];
        }

        // Toggles a tile between flagged and unknown state
        public void ToggleFlag(int row, int column)
        {
            // Maybe try to make this more elegant...
            if (GetTileState(row, column) == TileState.Unknown)
            {
                SetTileState(row, column, TileState.Flagged);
            }

            if (GetTileState(row, column) == TileState.Flagged)
            {
                SetTileState(row, column, TileState.Unknown);
            }
        }
    }
}