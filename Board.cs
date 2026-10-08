namespace _
{
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
}