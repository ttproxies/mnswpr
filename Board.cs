namespace mnswpr
{
    class Board
    {
        private const int spawnRadius = 2;

        public readonly int width, height, mines;
        private readonly Tile[] boardTiles;
        private readonly bool[] boardMines;

        public Board(int width, int height, int mines)
        {
            this.width = width;
            this.height = height;
            this.mines = mines;
            this.boardTiles = new Tile[width * height];
            this.boardMines = new bool[width * height];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    boardTiles[i * width + j] = new((j, i), 0, false);
                }
            }
        }

        // Distibutes mines across the board and sets boardMines accordingly
        public void GenerateMines(Utility.Coordinates clickCoords)
        {
            Random rand = new();
            int clickIndex = Utility.CoordsToInd(clickCoords, this.width);
            int[] pickedIndices = new int[this.mines];

            int curIndex;
            for (int i = 0; i < this.mines; i++)
            {
                do
                {
                    curIndex = rand.Next(this.width * this.height);
                }
                while (pickedIndices.Contains(curIndex) || IsInRadiusProximity(curIndex, clickIndex));
                this.boardTiles[curIndex].SetTileMine();
            }
        }

        // Returns whether targInd is in spawnRadius distance of relInd
        private bool IsInRadiusProximity(int relInd, int targInd)
        {
            var (relY, relX) = Utility.IndToCoords(relInd, this.width);
            var (targY, targX) = Utility.IndToCoords(targInd, this.height);

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
                    curInd = Utility.CoordsToInd(row + i, col + j, this.width);
                    if (IsAdjacent(curInd, Utility.CoordsToInd(row, col, this.width), i, j) && this.boardMines[curInd])
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
            int ind = Utility.CoordsToInd(row, col, this.width);
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
                        adjInd = ind + Utility.CoordsToInd(i, j, this.width);
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
            var (nextRow, nextCol) = Utility.IndToCoords(nextInd, this.width);
            Flood(nextRow, nextCol, visited, queue);
        }

        // Displays the current state of the board
        public void DisplayBoard()
        {
            Console.WriteLine("\x1b[3J");
            Console.Clear();
            int neighboringMines;
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {
                    switch (this.boardTiles[i * width + j].CurrentState)
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
                if (this.boardTiles[i] == TileState.Flagged)
                {
                    this.boardTiles[i] = TileState.Unknown;
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
            this.boardTiles[this.w * row + column] = state;
        }

        // Returns the TileState value of a board tile
        public TileState GetTileState(int row, int column)
        {
            // Console.WriteLine($"{row}, {column}, {this.w * row + column}");
            return this.boardTiles[this.w * row + column];
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