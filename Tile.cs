namespace mnswpr
{
    class Tile(Utility.Coordinates coords)
    {
        public enum TileState
        {
            Unknown,
            Revealed,
            Flagged,
            Exploded
        }

        public Utility.Coordinates Coords { get; } = coords;
        public TileState CurrentState { get; set; } = TileState.Unknown;
        public bool IsMine { get; set; } = false;
        public int AdjacentMines { get; set; } = 0;
    }
}