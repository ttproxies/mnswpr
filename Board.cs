namespace mnswpr
{
    class Board
    {
        public int Width { get; init; }
        public int Height { get; init; }
        public int Mines { get; init; }

        private Tile[] BoardTiles { get; init; }

        public Board(int width, int height, int mines)
        {
            Width = width;
            Height = height;
            Mines = mines;
            BoardTiles = new Tile[width * height];

            for (int i = 0; i < width * height; i++)
            {
                BoardTiles[i] = new Tile(IndToCoords(i));
                foreach (Utility.Coordinates adjacentCoords in Utility.ADJACENT_COORD_OFFSETS)
                {
                    if (BoardTiles[i].Coords + adjacentCoords)
                    {
                        
                    }
                }
            }
        }

        // Distibutes mines across the board and sets boardMines accordingly
        public void GenerateMines(Utility.Coordinates clickCoords)
        {
            Random rand = new();
            int clickIndex = CoordsToInd(clickCoords);
            int[] pickedIndices = new int[Mines];

            int curIndex;
            for (int i = 0; i < Mines; i++)
            {
                do
                {
                    curIndex = rand.Next(Width * Height);
                }
                while (pickedIndices.Contains(curIndex) || IsInRadiusProximity(curIndex, clickIndex));
                BoardTiles[curIndex].IsMine = true;
            }
        }

        // Returns number of neighboring mines relative to coords
        private int CountAdjacent(Utility.Coordinates coords, Tile.TileState tileState)
        {
            int stateCount = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int curInd = Utility.CoordsToInd(new(row + i, col + j), Width);
                    if (IsAdjacent(curInd, Utility.CoordsToInd(new(row, col), Width), i, j) && BoardTiles[curInd].IsMine)
                    {
                        mines++;
                    }
                }
            }

            return mines;
        }

        // Reveal all neighboring blank tiles recursively
        public void Flood(Utility.Coordinates coords, List<int>? visited = null, Queue<int>? queue = null)
        {
            visited ??= [];
            queue ??= [];

            // Mark current index as visited and uncover tile
            int ind = Utility.CoordsToInd(coords, Width);
            visited.Add(ind);
            if ( != TileState.Flagged)
            {
                SetTileState(row, col, TileState.Uncovered);
            }

            // Add neighbors to queue if not an edge tile
            if (MinesInProximity(row, col) > 0) return;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int adjInd = ind + CoordsToInd(new(i, j));
                    if (!visited.Contains(adjInd) && !queue.Contains(adjInd) && IsAdjacent(adjInd, ind, i, j))
                    {
                        queue.Enqueue(adjInd);
                    }
                }
            }


            if (queue.Count == 0)
            {
                return;
            }

            int nextInd = queue.Dequeue();
            var (nextRow, nextCol) = Utility.IndToCoords(nextInd, Width);
            Flood(nextRow, nextCol, visited, queue);
        }

        // Displays the current state of the board
        public void DisplayBoard()
        {
            Console.WriteLine("\x1b[3J");
            Console.Clear();
            int neighboringMines;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    switch (BoardTiles[i * width + j].CurrentState)
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
                if (BoardTiles[i] == TileState.Flagged)
                {
                    BoardTiles[i] = TileState.Unknown;
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
            BoardTiles[this.w * row + column] = state;
        }

        // Returns (row, col) coordinates for ind
        public Utility.Coordinates IndToCoords(int ind)
        {
            return new((int)(ind / Width), ind % Width);
        }

        // Returns index for (row, col) coordinates
        public int CoordsToInd(Utility.Coordinates coords)
        {
            return coords.Y * Width + coords.X;
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