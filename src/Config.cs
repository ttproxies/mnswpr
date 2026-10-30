namespace mnswpr
{
    class Config
    {
        public sealed record BoardPreset(string Name, int Width, int Height, int Mines)
        {
            // Presets (implement 'Custom' by creating object externally)
            public static readonly BoardPreset Beginner = new("Beginner", 9, 9, 10);
            public static readonly BoardPreset Intermediate = new("Intermediate", 16, 16, 40);
            public static readonly BoardPreset Expert = new("Expert", 30, 16, 99);
            public static readonly BoardPreset Superhuman = new("Superhuman", 50, 40, 500);

            // Container
            public static readonly IReadOnlyList<BoardPreset> All = [Beginner, Intermediate, Expert, Superhuman];

            // Resolver
            public static readonly Dictionary<string, BoardPreset> NAME_TO_PRESET = new()
            {
                {"Beginner", Beginner},
                {"Intermediate", Intermediate},
                {"Expert", Expert},
                {"Superhuman", Superhuman},
            };
        }

        public static string TITLE_ASCII = """
        ┌┬┐┌┐┌┌─┐┬ ┬┌─┐┬─┐
        ││││││└─┐│││├─┘├┬┘
        ┴ ┴┘└┘└─┘└┴┘┴  ┴└─                                                
        """;

        public static string ASCII_BORDER = """
        ───────────────────────────────────────────────────────
        """;

        public static readonly Dictionary<Tile.TileState, char> TILE_ICONS = new()
        {
            {Tile.TileState.Exploded, '*'},
            {Tile.TileState.Flagged, 'F'},
            {Tile.TileState.Revealed, '.'},
            {Tile.TileState.Unknown, '#'},
        };

        public static readonly Dictionary<Tile.TileState, string> ICON_COLORS = new()
        {
            {Tile.TileState.Exploded, "255;0;0"},
            {Tile.TileState.Flagged, "255;0;0"},
            {Tile.TileState.Revealed, "128;128;128"},
            {Tile.TileState.Unknown, "255;255;255"}, 
        };

        public static readonly Dictionary<Utility.Coordinates, string> CURSOR_OFFSET_ESCAPES = new()
        {
            {new(1, 0), "\e[2C"}, // Cursor forward
            {new(0, 1), "\e[B"}, // Cursor down
        };

        public static string CURSOR_RESET = "\e[H";
        public static string CURSOR_SAVE = "\e[s";
        public static string CURSOR_LOAD = "\e[u";
        public static string COLOR_PREFIX = "\e[38;2;";
        public static string COLOR_RESET = "\e[0m";
        public static string[] NUM_COLORS = ["0;33;255", "0;123;1", "206;0;0", "0;7;134", "129;0;0", "0;110;137", "20;20;20", "130;130;130"];

    }
}