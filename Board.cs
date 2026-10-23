using System.Collections;
using System.Diagnostics.Tracing;
using System.Globalization;
using Microsoft.VisualBasic;

namespace mnswpr
{
    class Board
    {
        public int Width { get; init; }
        public int Height { get; init; }
        public int Mines { get; init; }

        private readonly GameManager _overseer;

        // Coordinate-indexable array
        private Utility.TileMatrix BoardTiles { get; init; }

        // Return BoardTiles values
        public Tile this[Utility.Coordinates coords]
        {
            get => BoardTiles[coords];
            set => BoardTiles[coords] = value;
        }

        public Board(GameManager overseer, int width, int height, int mines)
        {
            (_overseer, Width, Height, Mines) = (overseer, width, height, mines);
            BoardTiles = new(Width, Height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Utility.Coordinates iterCoords = new(x, y);
                    BoardTiles[iterCoords] = new Tile(iterCoords);
                }
            }
        }

        private IEnumerable<Utility.Coordinates> GetAdjacentCoordinates(Utility.Coordinates relCoords)
        {
            foreach (Utility.Coordinates offsetCoordinates in Utility.ADJACENT_COORD_OFFSETS)
            {
                // Bound checking
                if (IsAdjacent(relCoords, relCoords + offsetCoordinates))
                    yield return relCoords + offsetCoordinates;
            }
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

                BoardTiles[mineCoords].IsMine = true;

                // Increment all adjacent tiles mine counts
                foreach (Utility.Coordinates adjacentCoords in GetAdjacentCoordinates(mineCoords))
                    BoardTiles[adjacentCoords].AdjacentMines++;
            }
        }

        // Returns number of neighboring mines relative to coords
        private int CountAdjacent(Utility.Coordinates coords, Tile.TileState tileState)
        {
            int stateCount = 0;
            foreach (Utility.Coordinates adjacentCoords in GetAdjacentCoordinates(coords))
                if (BoardTiles[adjacentCoords].CurrentState == tileState) stateCount++;

            return stateCount;
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

            // Return if nothing left in queue
            if (queue.Count == 0) return;

            // Otherwise, go to next tile in queue
            Utility.Coordinates nextCoords = queue.Dequeue();
            Flood(nextCoords, visited, queue);
        }

        public void Chord(Utility.Coordinates coords)
        {
            foreach (Utility.Coordinates adjacentCoords in GetAdjacentCoordinates(coords))
            {
                if (BoardTiles[coords].CurrentState == Tile.TileState.Unknown) Flood(coords);
            }
        }

        // Displays the current state of the board
        public void DisplayBoard()
        {
            Console.WriteLine("\x1b[3J");
            Console.Clear();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Tile boardTile = BoardTiles[new(x, y)];

                    switch (boardTile.CurrentState)
                    {
                        case Tile.TileState.Unknown:
                            Console.Write("◩ ");
                            break;

                        case Tile.TileState.Revealed:
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

        public void RevealTile(Utility.Coordinates coords)
        {   
            if (BoardTiles[coords].CurrentState == Tile.TileState.Flagged) return;
            if (BoardTiles[coords].IsMine) _overseer.State = GameManager.GameState.Exploded;

            BoardTiles[coords].CurrentState = Tile.TileState.Revealed;
        }

        // Toggles a tile between flagged and unknown state
        public void ToggleFlag(Utility.Coordinates coords)
        {
            Tile tile = BoardTiles[coords];

            // Pretty code courtesy of Claude :3 (C#8 switch expressions, could otherwise use a normal switch or conditional)
            tile.CurrentState = tile.CurrentState switch
            {
                Tile.TileState.Unknown => Tile.TileState.Flagged,
                Tile.TileState.Flagged => Tile.TileState.Unknown,
                var other              => other
            };
        }
    }
}