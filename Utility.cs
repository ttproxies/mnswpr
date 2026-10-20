namespace mnswpr
{
    class Utility
    {
        public readonly record struct Coordinates(int X, int Y);

        public enum PlayerAction
        {
            Flag,
            Uncover,
            Chord
        }

        public static Dictionary<char, PlayerAction> actionCharToEnum = new()
        {
            { 'U', PlayerAction.Uncover },
            { 'F', PlayerAction.Flag },
            { 'C', PlayerAction.Chord }
        };

        // Returns (row, col) coordinates for ind
        public static Coordinates IndToCoords(int ind, int boardWidth)
        {
            return new((int)(ind / boardWidth), ind % boardWidth);
        }

        // Returns index for (row, col) coordinates
        public static int CoordsToInd(Coordinates coords, int w)
        {
            return coords.Y * w + coords.X;
        }

        public static bool IsActionValid(string actionStr, int boardWidth, int boardHeight)
        {
            string[] actionParams = actionStr.Trim().Split();
            if (actionStr.Split().Length != 3)
            {
                return false;
            }
            if (actionParams[0].Length != 1)
            {
                return false;
            }

            char actionType = Convert.ToChar(actionParams[0]);
            int[] actionCoords;
            try
            {
                actionCoords = [Convert.ToInt32(actionParams[1]), Convert.ToInt32(actionParams[2])];
            }
            catch (Exception)
            {
                return false;
            }

            if (!actionCharToEnum.ContainsKey(actionType))
            {
                return false;
            }

            if (actionCoords[0] < 0 || actionCoords[0] >= boardWidth)
            {
                return false;
            }

            if (actionCoords[1] < 0 || actionCoords[1] >= boardHeight)
            {
                return false;
            }

            return true;
        }
    }
}