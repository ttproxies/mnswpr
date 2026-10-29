using System.Collections;
using System.Drawing;
using System.Text.RegularExpressions;

namespace mnswpr
{
    class Utility
    {
        // ---- UTILITY CLASSES ----

        // Defines an IEnumerable indexable with Utility.Coordinates for convenience
        public class TileMatrix(int matrixWidth, int matrixHeight) : IEnumerable<Tile>
        {
            private readonly Tile[] tilesArr = new Tile[matrixWidth * matrixHeight];
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

            // Base implementation required by the interface
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public readonly record struct Coordinates(int CoordX, int CoordY)
        {
            public int X { get; } = CoordX;
            public int Y { get; } = CoordY;

            public static Coordinates operator +(Coordinates operandLeft, Coordinates operandRight)
                => new(operandLeft.X + operandRight.X, operandLeft.Y + operandRight.Y);
        }

        public static readonly Coordinates[] ADJACENT_COORD_OFFSETS = [
            // Above
            new Coordinates(-1, -1),
            new Coordinates(0, -1),
            new Coordinates(1, -1),

            // Sides
            new Coordinates(-1, 0),
            new Coordinates(1, 0),

            // Below
            new Coordinates(-1, 1),
            new Coordinates(0, 1),
            new Coordinates(1, 1),
        ];

        // ---- INPUT HANDLING ----

        public enum PlayerAction
        {
            Flag,
            Reveal,
            Chord
        }

        // Dictionary for resolving action characters to enum values
        public static Dictionary<char, PlayerAction> ACTION_CHAR_TO_ENUM = new()
        {
            { 'R', PlayerAction.Reveal },
            { 'F', PlayerAction.Flag },
            { 'C', PlayerAction.Chord }
        };

        // Queries for user input (Action describes a method -> no ret. value)
        public static void GetAction(string prompt, string pattern, Func<string, bool> actionFilter, Action<string> actionHandler)
        {
            Console.Write($"\n{prompt}\n> ");
            string? input = Console.ReadLine();

            while (string.IsNullOrEmpty(input) || !Regex.IsMatch(input, pattern) || !actionFilter(input))
            {
                Console.Write($"\nInvalid input, please try again. {prompt}\n> ");
                input = Console.ReadLine();
            }

            actionHandler(input);
        }

        // Unwieldy overload for no filter
        public static void GetAction(string prompt, string pattern, Action<string> actionHandler)
        {
            GetAction(prompt, pattern, prompt => true, actionHandler);
        }

        public static readonly string GAME_ACTION_PATTERN = $@"^(?i)({string.Join('|', Utility.ACTION_CHAR_TO_ENUM.Keys)})\s+([1-9]\d*)\s+([1-9]\d*)$";
        public static readonly string PRESET_CHOICE_PATTERN = $@"^(?i)({string.Join('|', Config.BoardPreset.NAME_TO_PRESET.Keys)}|Custom)$";
        public static readonly string CUSTOM_PRESET_PATTERN = $@"^([1-9]\d*)\s+([1-9]\d*)\s+([1-9]\d*)$";
        public static readonly string RETRY_PATTERN = @"^(?i)[yn]$";


        public static string[] SplitRegex(string input) => Regex.Split(input, @"\s+");

        // ---- MISC ----

        public static void PrintTileColor(Tile tile)
        {
            if (tile.CurrentState == Tile.TileState.Revealed && tile.AdjacentMines > 0)
                Console.Write(Config.COLOR_PREFIX + Config.NUM_COLORS[tile.AdjacentMines - 1] + 'm' + tile.AdjacentMines + Config.COLOR_RESET);
            else
                Console.Write(Config.COLOR_PREFIX + Config.ICON_COLORS[tile.CurrentState] + 'm' + Config.TILE_ICONS[tile.CurrentState] + Config.COLOR_RESET);
        }


    }
}