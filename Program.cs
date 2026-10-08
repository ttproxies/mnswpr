using System;

namespace _
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
                playing = HandleAction(actType, actRow, actCol, board);
            }

            board.GenerateMines(actRow, actCol);
            board.RemoveStarterFlags();

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

    class Board
    {
        public enum TileState
        {
            Unknown,
            Uncovered,
            Flagged,
            Exploded
        }

        private const int spawnRadius = 2;

        public readonly int w, h, mines;
        private readonly TileState[] boardStates;
        private readonly bool[] boardMines;

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
        public void GenerateMines(int clickRow, int clickColumn)
        {
            Random rand = new();
            int clickIndex = Utility.CoordsToInd(clickRow, clickColumn, this.w);
            int[] pickedIndices = new int[this.mines];

            int curIndex;
            for (int i = 0; i < this.mines; i++)
            {
                do
                {
                    curIndex = rand.Next(this.w * this.h);
                }
                while (pickedIndices.Contains(curIndex) || IsInRadiusProximity(curIndex, clickIndex));
                this.boardMines[curIndex] = true;
            }
        }

        // Returns whether targInd is in spawnRadius distance of relInd
        private bool IsInRadiusProximity(int relInd, int targInd)
        {
            var (relY, relX) = Utility.IndToCoords(relInd, this.w);
            var (targY, targX) = Utility.IndToCoords(targInd, this.w);

            return targX >= relX - spawnRadius && targX <= relX + spawnRadius &&
                    targY >= relY - spawnRadius && targY <= relY + spawnRadius;
        }

        // Returns number of neighboring mines
        private int MinesInProximity(int row, int col)
        {
            int mines = 0;
            int curInd;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    curInd = Utility.CoordsToInd(row + i, col + j, this.w);
                    if (IsAdjacent(curInd, Utility.CoordsToInd(row, col, this.w), i, j) && this.boardMines[curInd])
                    {
                        mines++;
                    }
                }
            }

            return mines;
        }

        // Reveal all neighboring blank tiles recursively
        public void Flood(int row, int col, List<int>? visited = null, Queue<int>? queue = null)
        {
            visited ??= [];
            queue ??= [];

            // Mark current index as visited and uncover tile
            int ind = Utility.CoordsToInd(row, col, this.w);
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
                        adjInd = ind + Utility.CoordsToInd(i, j, this.w);
                        if (!visited.Contains(adjInd) && !queue.Contains(adjInd) && IsAdjacent(adjInd, ind, i, j))
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
            var (nextRow, nextCol) = Utility.IndToCoords(nextInd, this.w);
            Flood(nextRow, nextCol, visited, queue);
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
                            Console.Write(neighboringMines > 0 ? neighboringMines + " " : "⚬ ");
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

        // Returns whether adjInd is actually adjacent to relInd for given dy and dx offsets
        private bool IsAdjacent(int adjInd, int relInd, int dy, int dx)
        {
            // Trivial bounds checking
            if (adjInd < 0 || adjInd >= this.w * this.h)
            {
                return false;
            }

            // Evil freaking wizardry (check for wrapping)
            if (adjInd % this.w != relInd % this.w + dx || (int)(adjInd / this.w) != (int)(relInd / this.w) + dy)
            {
                return false;
            }

            return true;
        }

        // Removes all flags placed before mines were generated
        public void RemoveStarterFlags()
        {
            for (int i = 0; i < this.w * this.h; i++)
            {
                if (this.boardStates[i] == TileState.Flagged)
                {
                    this.boardStates[i] = TileState.Unknown;
                }
            }
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
            // Console.WriteLine($"{row}, {column}, {this.w * row + column}");
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
            if (GetTileState(row, column) == TileState.Unknown)
            {
                SetTileState(row, column, TileState.Flagged);
            }
            else if (GetTileState(row, column) == TileState.Flagged)
            {
                SetTileState(row, column, TileState.Unknown);
            }
        }
    }

    class Utility
    {
        // Returns (row, col) coordinates for ind
        public static (int, int) IndToCoords(int ind, int w)
        {
            return ((int)(ind / w), ind % w);
        }

        public static int CoordsToInd(int row, int col, int w)
        {
            return row * w + col;
        }
    }
}