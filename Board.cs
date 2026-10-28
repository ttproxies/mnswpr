namespace mnswpr
{
    class Board
    {
        public int Width { get; init; }
        public int Height { get; init; }
        public int Mines { get; init; }

        // Coordinate-indexable array
        public Utility.TileMatrix BoardTiles { get; init; }

        public Board(int width, int height, int mines)
        {
            (Width, Height, Mines) = (width, height, mines);
            BoardTiles = new(Width, Height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Utility.Coordinates iterCoords = new(x, y);
                    BoardTiles[iterCoords] = new(iterCoords);
                }
            }
        }

        private IEnumerable<Utility.Coordinates> GetAdjacentCoordinates(Utility.Coordinates relCoords)
        {
            foreach (Utility.Coordinates offsetCoordinates in Utility.ADJACENT_COORD_OFFSETS)
            {
                // Bound checking
                if (IsAdjacent(relCoords, relCoords + offsetCoordinates))
                    yield return relCoords + offsetCoordinates;            }
        }

        // Distibutes mines across the board and sets boardMines accordingly
        public void GenerateMines(Utility.Coordinates clickCoords)
        {
            Random rand = new();
            var pickedCoordinates = new Utility.Coordinates[Mines];

            Utility.Coordinates mineCoords;
            for (int i = 0; i < Mines; i++)
            {
                // Choose a random index to place the mine
                do { mineCoords = new(rand.Next(Width), rand.Next(Height)); }
                while (pickedCoordinates.Contains(mineCoords) || IsAdjacent(mineCoords, clickCoords));

                pickedCoordinates[i] = mineCoords;

                foreach (Utility.Coordinates adjacentCoords in GetAdjacentCoordinates(mineCoords))
                    BoardTiles[adjacentCoords].AdjacentMines++;

                BoardTiles[mineCoords].IsMine = true;
            }
        }

        // Reveal all neighboring blank tiles recursively TODO: rewrite using coordinates if possible
        public void Flood(Utility.Coordinates coords, List<Utility.Coordinates>? visited = null, Queue<Utility.Coordinates>? queue = null)
        {
            Tile coordsTile = BoardTiles[coords];

            // Init on first iteration
            visited ??= []; queue ??= [];

            // Mark current index as visited and reveal tile
            visited.Add(coords);
            RevealTile(coords);

            // Add neighbors to queue if not an edge tile
            if (coordsTile.AdjacentMines > 0) return;

            foreach (Utility.Coordinates adjacentCoords in GetAdjacentCoordinates(coords))
            {
                if (!visited.Contains(adjacentCoords)
                 && !queue.Contains(adjacentCoords)
                 && IsAdjacent(coords, adjacentCoords))
                {
                    queue.Enqueue(adjacentCoords);
                }
            }

            // Pick out tiles until none is left in queue
            while (queue.Count != 0)
            {
                Utility.Coordinates nextCoords = queue.Dequeue();
                Flood(nextCoords, visited, queue);
            }
        }

        public void Chord(Utility.Coordinates coords)
        {
            foreach (Utility.Coordinates adjacentCoords in GetAdjacentCoordinates(coords))
                if (BoardTiles[adjacentCoords].CurrentState == Tile.TileState.Unknown)
                {
                    Flood(adjacentCoords);
                }
        }

        // Displays the current state of the board
        public void DisplayBoard()
        {
            Console.WriteLine();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Tile boardTile = BoardTiles[new(x, y)];

                    if (boardTile.CurrentState == Tile.TileState.Revealed && boardTile.AdjacentMines > 0)
                        Console.Write(boardTile.AdjacentMines);
                    else Console.Write(Config.TILE_ICONS[boardTile.CurrentState]);

                    Console.Write(" ");
                }

                Console.WriteLine();
            }
        }

        // Returns whether targCoords is adjacent to relCoords
        private bool IsAdjacent(Utility.Coordinates relCoords, Utility.Coordinates targCoords)
        {
            // Bounds checking
            if (targCoords.X < 0 || targCoords.X >= Width
             || targCoords.Y < 0 || targCoords.Y >= Height) return false;

            // Adjacency
            if (Math.Abs(relCoords.X - targCoords.X) > 1
             || Math.Abs(relCoords.Y - targCoords.Y) > 1) return false;

            return true;
        }

        private bool IsAdjacent(int relInd, int targInd)
        {
            return IsAdjacent(IndToCoords(relInd), IndToCoords(targInd));
        }

        // Returns number of adjacent cells that satisfy comparison. Very overkill, but I get to use Func<>
        public int CountAdjacent(Utility.Coordinates coords, Tile.TileState filterState)
        {
            int count = 0;
            foreach (Utility.Coordinates adjacentCoords in GetAdjacentCoordinates(coords))
                if (BoardTiles[adjacentCoords].CurrentState == filterState) count++;

            return count;
        }

        // Sets all tiles with a mine to TileState.Exploded
        public void ExplodeMines()
        {
            foreach (Tile boardTile in BoardTiles)
                if (boardTile.IsMine) boardTile.CurrentState = Tile.TileState.Exploded;
        }

        // Returns (row, col) coordinates for ind
        public Utility.Coordinates IndToCoords(int ind) => new((int)(ind / Width), ind % Width);

        public void RevealTile(Utility.Coordinates coords)
        {
            Tile tile = BoardTiles[coords];
            tile.CurrentState = tile.CurrentState switch
            {
                Tile.TileState.Unknown => tile.IsMine ? Tile.TileState.Exploded : Tile.TileState.Revealed,
                var other => other
            };
        }

        // Toggles a tile between flagged and unknown state
        public void ToggleFlag(Utility.Coordinates coords)
        {
            Tile tile = BoardTiles[coords];

            tile.CurrentState = tile.CurrentState switch
            {
                Tile.TileState.Flagged => Tile.TileState.Unknown,
                Tile.TileState.Unknown => Tile.TileState.Flagged,
                var other => other
            };
        }
    }
}