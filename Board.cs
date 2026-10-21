namespace mnswpr
{
    class Board
    {
        public int Width { get; init; }
        public int Height { get; init; }
        public int Mines { get; init; }

        // Coordinate-indexable array
        private Utility.TileMatrix BoardTiles { get; init; }

        public Board(int width, int height, int mines)
        {
            Width = width;
            Height = height;
            Mines = mines;
            BoardTiles = new(Width, Height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Utility.Coordinates iterCoords = new(x, y);
                    BoardTiles[iterCoords] = new Tile(iterCoords);
                    foreach (Utility.Coordinates offsetCoords in Utility.ADJACENT_COORD_OFFSETS)
                    {
                        // Cache adjacent coordinates if valid
                        Utility.Coordinates adjacentCoords = iterCoords + offsetCoords;

                        if (IsAdjacent(iterCoords, offsetCoords))
                        {
                            BoardTiles[iterCoords].AdjacentIndexes.Add(CoordsToInd(adjacentCoords));
                        }
                    }
                }
            }
        }

        // Define an indexer for interpreting Coordinates as valid indices
        public Tile this[Utility.Coordinates]
        {
            get =>
        }

        // Distibutes mines across the board and sets boardMines accordingly
        public void GenerateMines(Utility.Coordinates clickCoords)
        {
            Random rand = new();
            int clickIndex = CoordsToInd(clickCoords);
            int[] pickedIndexes = new int[Mines];

            int mineIndex;
            for (int i = 0; i < Mines; i++)
            {
                // Choose a random index to place the mine
                do { mineIndex = rand.Next(Width * Height); }
                while (pickedIndexes.Contains(mineIndex) || IsAdjacent(mineIndex, clickIndex));

                BoardTiles[mineIndex].IsMine = true;

                // Increment all adjacent tiles mine counts
                foreach (int adjacentInd in BoardTiles[mineIndex].AdjacentIndexes)
                {
                    BoardTiles[adjacentInd].AdjacentMines++;
                }
            }
        }

        // Returns number of neighboring mines relative to coords
        private int CountAdjacent(Utility.Coordinates coords, Tile.TileState tileState)
        {
            int stateCount = 0;
            foreach (int adjacentInd in BoardTiles[CoordsToInd(coords)].AdjacentIndexes)
            {
                if (BoardTiles[adjacentInd].CurrentState == tileState) stateCount++;
            }

            return stateCount;
        }

        // Reveal all neighboring blank tiles recursively TODO: rewrite using coordinates if possible
        public void Flood(Utility.Coordinates coords, List<int>? visited = null, Queue<int>? queue = null)
        {
            int coordsInd = CoordsToInd(coords);
            Tile coordsTile = BoardTiles[coordsInd];
            // Init on first iteration
            visited ??= []; queue ??= [];

            // Mark current index as visited and uncover tile
            visited.Add(coordsInd);
            if (coordsTile.CurrentState != Tile.TileState.Flagged)
            {
                coordsTile.CurrentState = Tile.TileState.Uncovered;
            }

            // Add neighbors to queue if not an edge tile
            if (coordsTile.AdjacentMines > 0) return;

            foreach (int adjacentInd in coordsTile.AdjacentIndexes)
            {
                if (!visited.Contains(adjacentInd)
                 && !queue.Contains(adjacentInd)
                 && IsAdjacent(coordsInd, adjacentInd))
                {
                    queue.Enqueue(adjacentInd);
                }
            }

            if (queue.Count == 0) return;

            int nextInd = queue.Dequeue();
            Flood(IndToCoords(nextInd), visited, queue);
        }

        // Displays the current state of the board
        public void DisplayBoard()
        {
            Console.WriteLine("\x1b[2J");
            Console.Clear();

            foreach (Tile boardTile in BoardTiles)
            {
                switch (boardTile.CurrentState)
                {
                    case Tile.TileState.Unknown:
                        Console.Write("◩ ");
                        break;

                    case Tile.TileState.Uncovered:
                        Console.Write(boardTile.AdjacentMines > 0 ? boardTile.AdjacentMines + " " : "⚬ ");
                        break;

                    case Tile.TileState.Flagged:
                        Console.Write("⚐ ");
                        break;

                    case Tile.TileState.Exploded:
                        Console.Write("✴︎ ");
                        break;
                }
            }
        }


        // Returns whether targCoords is adjacent to relCoords
        private bool IsAdjacent(Utility.Coordinates relCoords, Utility.Coordinates targCoords)
        {
            // Bounds checking
            if (targCoords.X < 0 || targCoords.X >= Width
             || targCoords.Y < 0 || targCoords.Y >= Height) return false;

            // Adjacency
            if (Math.Abs(relCoords.X - targCoords.X) != 1
             || Math.Abs(relCoords.Y - targCoords.Y) != 1) return false;

            return true;
        }

        private bool IsAdjacent(int relInd, int targInd)
        {
            return IsAdjacent(IndToCoords(relInd), IndToCoords(targInd));
        }

        // Sets all tiles with a mine to TileState.Exploded
        public void ExplodeMines()
        {
            foreach (Tile boardTile in BoardTiles)
            {
                if (boardTile.IsMine) boardTile.CurrentState = Tile.TileState.Exploded;
            }
        }

        // Returns (row, col) coordinates for ind
        public Utility.Coordinates IndToCoords(int ind)
        {
            return new((int)(ind / Width), ind % Width);
        }

        // Returns index for (row, col) coordinates
        public int CoordsToInd(Utility.Coordinates coords)
        {

        }


        // Toggles a tile between flagged and unknown state
        public void ToggleFlag(Utility.Coordinates)
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