using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;

namespace mnswpr
{
    class GameManager
    {
        // Properties
        public bool Playing { get; set; } = true;
        public GameState State { get; set; } = GameState.Idling;
        public Config.BoardPreset ActivePreset { get; private set; }

        // Fields
        private readonly List<Utility.Coordinates> _flagCoords = [];
        private Utility.Coordinates _selectionCoords = new(0, 0);
        private Utility.Coordinates _actionCoords;
        private Utility.PlayerAction _actionType;
        private int _flaggedMines = 0;

        private readonly int _boardWidth;
        private readonly int _boardHeight;
        private readonly int _mines;
        private readonly Board _board;

        // Input validation

        public enum GameState
        {
            Idling, // Player has not yet revealed any tiles
            Playing, // Round in progress
            Exploded, // Player has exploded
            Completed // Player has won
        }

        public GameManager(Config.BoardPreset preset)
        {
            (_boardWidth, _boardHeight, _mines) = (preset.Width, preset.Height, preset.Mines);
            _board = new(_boardWidth, _boardHeight, _mines);
            ActivePreset = preset;
        }

        // No preset given
        public GameManager()
        {
            // Have the user choose a preset
            DisplayPresets();

            // Set active preset
            while (ActivePreset == null) Utility.GetAction("Preset name", Utility.PRESET_CHOICE_PATTERN, PresetHandler);

            // Want to keep readonly; cannot extract
            (_boardWidth, _boardHeight, _mines) = (ActivePreset.Width, ActivePreset.Height, ActivePreset.Mines);
            _board = new(_boardWidth, _boardHeight, _mines);
        }

        // Shows all available board presets
        private static void DisplayPresets()
        {
            Console.WriteLine("Welcome. Please choose a board preset to begin:\n");
            foreach (Config.BoardPreset preset in Config.BoardPreset.All)
            {
                Console.WriteLine($"{preset.Name} - {preset.Width}x{preset.Height}, {preset.Mines} mines");
            }

            Console.WriteLine("Additionally, you may set your attributes using the \"Custom\" preset.");
        }

        // Display board and update cursor selection
        public void DisplayBoard()
        {
            _board.DisplayBoard();
            Utility.UpdateCursorSelection(_selectionCoords);
        }

        // Updates board fields based on player input
        public void GetGameInput()
        {
            ConsoleKey pressed;
            while (!Utility.ACTION_PRESS_TO_ENUM.ContainsKey(pressed = Console.ReadKey().Key))
                // If selection is being and can be moved, move it
                if (Utility.SELECTION_MOVE_OFFSETS.TryGetValue(pressed, out Utility.Coordinates moveOffset)
                 && _board.IsAdjacent(_selectionCoords, _selectionCoords + moveOffset))
                {
                    _selectionCoords += moveOffset;
                    DisplayBoard();
                }

            // Perform action on selected
            _actionType = Utility.ACTION_PRESS_TO_ENUM[pressed];
            _actionCoords = _selectionCoords;
            DisplayBoard();

        }

        // Handle setting board preset 
        private void PresetHandler(string presetName)
        {
            // Ignore input case
            presetName = presetName.ToLower();

            if (presetName != "custom")
            {
                // Set preset to enum value if not custom
                ActivePreset = Config.BoardPreset.NAME_TO_PRESET[char.ToUpper(presetName[0]) + presetName[1..]];
            }
            else
            {
                // Query user for custom board attributes
                Utility.GetAction("Enter board dimensions and mine count [w h m]", Utility.CUSTOM_PRESET_PATTERN,

                // Filter
                (customPreset) =>
                {
                    // Filter invalid mine count
                    string[] presetArgs = Utility.SplitRegex(customPreset);
                    (int presetWidth, int presetHeight, int presetMines) = (
                        Convert.ToInt32(presetArgs[0]),
                        Convert.ToInt32(presetArgs[1]),
                        Convert.ToInt32(presetArgs[2])
                    );

                    if (presetWidth > Console.WindowWidth) { Console.WriteLine("!! Board width cannot exceed that of the console."); return false; }
                    if (presetHeight > Console.WindowHeight) { Console.WriteLine("!! Board height cannot exceed that of the console."); return false; }
                    if (presetMines > presetWidth * presetHeight - 9) { Console.WriteLine("!! Number of mines must be possible "); return false; }
                    return true;
                },

                // Handler
                (customPreset) =>
                {
                    // Set preset if valid
                    string[] presetArgs = Utility.SplitRegex(customPreset);
                    ActivePreset = new(
                        "Custom", // Name
                        Convert.ToInt32(presetArgs[0]), // Width
                        Convert.ToInt32(presetArgs[1]), // Height
                        Convert.ToInt32(presetArgs[2]) // Mines
                    );
                });
            }
        }

        // Show game over message
        public void ShowGameOver()
        {
            Console.WriteLine(Config.CURSOR_LOAD); // Jump to end
            if (State == GameState.Completed) Console.WriteLine("\nYou win! You've flagged every mine correctly.");
            else if (State == GameState.Exploded) Console.WriteLine("\nI'm sorry, you lose.. You've exploded a mine.");
        }

        // Performs whatever game command was given by user
        public void PerformUserAction()
        {
            Tile actionTile = _board.BoardTiles[_actionCoords];
            switch (_actionType)
            {
                case Utility.PlayerAction.Flag:
                    _board.ToggleFlag(_actionCoords);
                    // Toggle flag: .Remove and .Add automatically perform lookup
                    if (!_flagCoords.Remove(_actionCoords))
                    {
                        _flagCoords.Add(_actionCoords);
                        if (actionTile.IsMine) _flaggedMines++;
                    }
                    else if (actionTile.IsMine) _flaggedMines--;

                    if (_flaggedMines == _mines) State = GameState.Completed;

                    break;

                case Utility.PlayerAction.Reveal:
                    // Transition to next game state if idling TODO: make more elegant
                    if (State == GameState.Idling)
                        _board.GenerateMines(_actionCoords);
                    State = GameState.Playing;

                    if (!_flagCoords.Contains(_actionCoords) && actionTile.IsMine)
                    {
                        _board.ExplodeMines();
                        State = GameState.Exploded;
                    }
                    else _board.Flood(_actionCoords);

                    break;

                case Utility.PlayerAction.Chord:
                    // Think about ways to make more elegant
                    if (_board.CountAdjacent(_actionCoords, Tile.TileState.Flagged) >= actionTile.AdjacentMines)
                        _board.Chord(_actionCoords);

                    if (_board.CountAdjacent(_actionCoords, Tile.TileState.Exploded) > 0)
                    {
                        _board.ExplodeMines();
                        State = GameState.Exploded;
                    }

                    break;
            }
        }
    }
}
