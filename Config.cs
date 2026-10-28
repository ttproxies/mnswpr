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
            public static readonly BoardPreset Superhuman = new("Superhuman", 50, 50, 500);

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
        ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        """;

        public static readonly Dictionary<Tile.TileState, char> TILE_ICONS = new()
        {
            {Tile.TileState.Exploded, '*'},
            {Tile.TileState.Flagged, 'F'},
            {Tile.TileState.Revealed, '.'},
            {Tile.TileState.Unknown, '#'},
        };
    }
}