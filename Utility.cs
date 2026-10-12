namespace mnswpr
{
    class Utility
    {
        // Returns (row, col) coordinates for ind
        public static (int y, int x) IndToCoords(int ind, int w)
        {
            return ((int)(ind / w), ind % w);
        }

        // 
        public static int CoordsToInd(int row, int col, int w)
        {
            return row * w + col;
        }

        private static bool IsActionValid(string actionStr, int boardWidth, int boardHeight, string allowedActions)
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

            if (!allowedActions.Contains(actionType))
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