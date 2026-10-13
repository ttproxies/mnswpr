namespace mnswpr
{
    public enum TileState
    {
        Unknown,
        Uncovered,
        Flagged,
        Exploded
    }

    class Tile((int x, int y) coords, int neighboringMines, bool isMine)
    {
        TileState currentState = TileState.Unknown;
    }
}