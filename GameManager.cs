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

        // Fields
        private readonly List<Utility.Coordinates> _flagCoords = [];
        private Config.BoardPreset _activePreset;
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

        public GameManager()
        {
            // Have the user choose a preset
            Console.WriteLine("Welcome. Please choose a board preset to begin:\n");
            foreach (Config.BoardPreset preset in Config.BoardPreset.All)
            {
                Console.WriteLine($"{preset.Name} - {preset.Width}x{preset.Height}, {preset.Mines} mines");
            }

            Console.WriteLine("Additionally, you may set your attributes using the \"Custom\" preset.");

            // Set active preset
            while (_activePreset == null) Utility.GetAction("Preset name", Utility.PRESET_CHOICE_PATTERN, PresetHandler);

            (_boardWidth, _boardHeight, _mines) = (_activePreset.Width, _activePreset.Height, _activePreset.Mines);
            _board = new(_boardWidth, _boardHeight, _mines);
        }

        // Expose common methods while preserving encapsulation
        public void DisplayBoard() => _board.DisplayBoard();
        public void GetGameInput() => Utility.GetAction("Action", Utility.GAME_ACTION_PATTERN, GameActionFilter, GameActionHandler);

        // Handle setting board preset 
        private void PresetHandler(string presetName)
        {
            // Ignore input case
            presetName = presetName.ToLower();
            
            if (presetName != "custom")
            {
                // Set preset to enum value if not custom
                _activePreset = Config.BoardPreset.NAME_TO_PRESET[char.ToUpper(presetName[0]) + presetName[1..]];
            }
            else
            {
                // Query user for custom board attributes
                Utility.GetAction("Enter board dimensions and mine count", Utility.CUSTOM_PRESET_PATTERN,
                
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

                    if (presetWidth > Console.WindowWidth) return false;
                    if (presetMines > presetWidth * presetHeight - 9) return false;
                    return true;
                },

                // Handler
                (customPreset) =>
                {
                    // Set preset if valid
                    string[] presetArgs = Utility.SplitRegex(customPreset);
                    _activePreset = new (
                        "Custom", // Name
                        Convert.ToInt32(presetArgs[0]), // Width
                        Convert.ToInt32(presetArgs[1]), // Height
                        Convert.ToInt32(presetArgs[2]) // Mines
                    );
                });
            }
        }
        
        // Filter game action input
        private bool GameActionFilter(string action)
        {
            string[] actionArgs = Utility.SplitRegex(action);
            
            if (Convert.ToInt32(actionArgs[1]) > _boardWidth) return false;
            if (Convert.ToInt32(actionArgs[2]) > _boardHeight) return false;

            return true; // Note: regex already disallows unrecognized action types
        }

        // Handle game input
        private void GameActionHandler(string inputAction)
        {
            string[] inputParams = Utility.SplitRegex(inputAction);
            (_actionType, _actionCoords) =
            (
                // Action type
                Utility.ACTION_CHAR_TO_ENUM[Convert.ToChar(inputParams[0].ToUpper())],
                // Coordinates
                new
                (
                    Convert.ToInt32(inputParams[1]) - 1,
                    Convert.ToInt32(inputParams[2]) - 1
                )
            );
        }

        public void ShowGameOver()
        {
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
