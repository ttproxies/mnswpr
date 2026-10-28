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
        }

        public static string GAME_ASCII = """                                           
         _ __ ___  _ __  _____      ___ __  _ __ 
        | '_ ` _ \| '_ \/ __\ \ /\ / / '_ \| '__|
        | | | | | | | | \__ \\ V  V /| |_) | |   
        |_| |_| |_|_| |_|___/ \_/\_/ | .__/|_|   
                                     |_|         
        """;

        public static string ASCII_BORDER = """
        ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        """;
    }
}