namespace mnswpr
{
    class GameManager
    {
        // Properties
        public bool Playing { get; set; } = true;
        public GameState State { get; set; } = GameState.Idling;

        // Fields
        private readonly List<Utility.Coordinates> _flagCoords = [];
        private readonly Config.BoardPreset _activePreset;
        private Utility.Coordinates _actionCoords;
        private Utility.PlayerAction _actionType;
        private int _flaggedMines = 0;

        private readonly int _boardWidth;
        private readonly int _boardHeight;
        private readonly int _mines;
        private readonly Board _board;

        private readonly string _gameActionPattern;

        public enum GameState
        {
            Idling, // Player has not yet revealed any tiles
            Playing, // Round in progress
            Exploded, // Player has exploded
            Completed // Player has won
        }

        public GameManager()
        {
            Console.WriteLine(Config.GAME_ASCII);
            Console.WriteLine(Config.ASCII_BORDER);

            Console.WriteLine("Welcome. Please choose a board preset to begin: ");
            foreach (Config.BoardPreset preset in Config.BoardPreset.All)
            {
                Console.WriteLine($"{preset.Name} - {preset.Width}x{preset.Height}, {preset.Mines} mines");
            }
        }

        public void DisplayBoard() => _board.DisplayBoard();

        // Handler for Utility.GetAction
        public void GameActionHandler(string? inputAction)
        {
            while (string.IsNullOrEmpty(inputAction) || !Utility.IsActionValid(inputAction, _boardWidth, _boardHeight))
            {
                Console.Write("\nInvalid input, please try again:\n> ");
                inputAction = Console.ReadLine();
            }

            string[] inputParams = inputAction.Split();

            (_actionType, _actionCoords) =
            (
                // Action type
                Utility.ACTION_CHAR_TO_ENUM
                [
                    Convert.ToChar(inputParams[0])
                ],
                // Coordinates
                new
                (
                    Convert.ToInt32(inputParams[1]) - 1,
                    Convert.ToInt32(inputParams[2]) - 1
                )
            );
        }

        // TODO: make more elegant
        public void DoUserAction()
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

        // Determines whether game can continue
        public void GameStatusCheck()
        {

        }
    }
}
