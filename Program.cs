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
            char actType;
            int actCol, actRow;

            // Game init loop
            while (!playing)
            {
                board.DisplayBoard();

                (actType, actCol, actRow) = GetAction(board.w, board.h, allowedActions);
                if (actType == 'U')
                {
                    playing = true;
                    board.GenerateMines(actRow, actCol);
                    board.Flood(actRow, actCol);
                }
            }

            // TODO: remove all flags

            // Main game loop
            while (playing)
            {
                board.DisplayBoard();

                (actType, actCol, actRow) = GetAction(board.w, board.h, allowedActions);

                playing = HandleAction(actType, actRow, actCol, board);
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
            int clickIndex = clickRow * this.w + clickColumn;
            int[] pickedIndices = new int[this.mines];

            int curIndex;
            for (int i = 0; i < this.mines; i++)
            {
                do
                {
                    curIndex = rand.Next(this.w * this.h);
                }
                while (pickedIndices.Contains(curIndex) || !IsInRadiusProximity(curIndex, clickIndex));
                this.boardMines[curIndex] = true;
            }
        }

        private bool IsInRadiusProximity(int relInd, int targInd)
        {
            int relX = relInd % this.w, relY = (int)(relInd / this.w);
            int targX = relInd % this.w, targY = (int)(relInd / this.w);

            return (targX >= relX - spawnRadius && targX <= relX + spawnRadius) &&
                    (targY >= relY - spawnRadius && targY <= relY + spawnRadius);
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
                    curInd = (row + i) * this.w + (column + j);
                    if (curInd >= 0 && curInd < this.w * this.h && this.boardMines[curInd])
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
            int ind = this.w * row + col;
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
                            if (this.boardMines[i * w + j])
                            {
                                Console.Write("X ");
                            }
                            else
                            {
                                Console.Write("◩ ");
                            }
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

        // Returns whether an index is in range relative to original index
        private bool IsValidAdj(int adjInd, int relInd, int i, int j)
        {
            // Trivial bounds checking
            if (adjInd < 0 || adjInd >= this.w * this.h)
            {
                return false;
            }

            // Evil freaking wizardry (check for wrapping)
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