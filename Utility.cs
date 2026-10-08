namespace mnswpr
{
    class Utility
    {
        // Returns (row, col) coordinates for ind
        public static (int, int) IndToCoords(int ind, int w)
        {
            return ((int)(ind / w), ind % w);
        }

        public static int CoordsToInd(int row, int col, int w)
        {
            return row * w + col;
        }
    }
}