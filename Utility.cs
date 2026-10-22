using System.Collections;

namespace mnswpr
{
    class Utility
    {
        // Defines an IEnumerable indexable with Utility.Coordinates for convenience
        public class TileMatrix(int matrixWidth, int matrixHeight) : IEnumerable<Tile>
        {
            private readonly Tile[] tilesArr = [];
            public int Width { get; } = matrixWidth;
            public int Height { get; } = matrixHeight;

            public Tile this[Coordinates coords]
            {
                get => tilesArr[coords.Y * Width + coords.X];
                set => tilesArr[coords.Y * Width + coords.X] = value;
            }

            public IEnumerator<Tile> GetEnumerator()
            {
                foreach (Tile tile in tilesArr)
                    yield return tile;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public readonly record struct Coordinates(int CoordX, int CoordY)
        {
            public int X { get; } = CoordX;
            public int Y { get; } = CoordY;

            public static Coordinates operator +(Coordinates operandLeft, Coordinates operandRight)
                => new(operandLeft.X + operandRight.X, operandLeft.Y + operandRight.Y);
        }

        public static readonly Coordinates[] ADJACENT_COORD_OFFSETS = [
            new Coordinates(-1, -1),
            new Coordinates(0, -1),
            new Coordinates(1, -1),

            new Coordinates(-1, 0),
            new Coordinates(1, 0),

            new Coordinates(-1, 1),
            new Coordinates(0, 1),
            new Coordinates(1, 1),
        ];

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

        // Perform input checking TODO: write as regex
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