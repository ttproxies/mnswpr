namespace mnswpr
{
    public enum TileState
    {
        Unknown,
        Uncovered,
        Flagged,
        Exploded
    }

    class Tile(Utility.Coordinates coords, int neighboringMines, bool isMine) 
    {
        public Utility.Coordinates Coords { get; } = coords;
        public int NeighboringMines { get; } = neighboringMines;
        public bool IsMine { get;set; } = isMine;

        public TileState CurrentState { get; set; } = TileState.Unknown;
    }
} 