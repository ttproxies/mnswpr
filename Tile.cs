namespace mnswpr
{
    class Tile(Utility.Coordinates coords, int neighboringMines, bool isMine)
    {
        public enum TileState
        {
            Unknown,
            Uncovered,
            Flagged,
            Exploded
        }

        public Utility.Coordinates Coords { get; } = coords;
        public int NeighboringMines { get; } = neighboringMines;
        public bool IsMine { get; set; } = isMine;

        public TileState CurrentState { get; set; } = TileState.Unknown;
        public List<int> AdjacentIndexes { get; } = [];
    }
}