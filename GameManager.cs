namespace mnswpr
{
    class GameManager(int boardWidth, int boardHeight, int mines)
    { 
        // Properties
        public bool Playing { get; set; } = true;
        public GameState State { get; set; } = GameState.Idling;
        
        // Fields
        private readonly List<Utility.Coordinates> _flagCoords = [];
        private Utility.Coordinates _actionCoords;
        private Utility.PlayerAction _actionType;

        // Constructor
        private readonly int _boardWidth = boardWidth;
        private readonly int _boardHeight = boardHeight;
        private readonly int _mines = mines;
        private readonly Board _board = new(boardWidth, boardHeight, mines);

        public enum GameState
        {
            Idling, // Player has not yet revealed any tiles
            Playing, // Round in progress
            Exploded, // Player has exploded
            Completed // Player has won
        }

        // TODO: make more elegant
        public void DisplayBoard() => _board.DisplayBoard();

        // Queries for user input
        public void GetAction()
        {
            Console.Write("\nAction:\n> ");
            string? inputAction = Console.ReadLine();

            while (string.IsNullOrEmpty(inputAction) || !Utility.IsActionValid(inputAction, _boardWidth, _boardHeight))
            {
                Console.Write("\nInvalid input, please try again:\n> ");
                inputAction = Console.ReadLine();
            }

            string[] inputParams = inputAction.Split();

            (_actionType, _actionCoords) = (Utility.ACTION_CHAR_TO_ENUM[Convert.ToChar(inputParams[0])], new(Convert.ToInt32(inputParams[1])-1, Convert.ToInt32(inputParams[2])-1));
        }
    
        // TODO: make more elegant
        public void HandleAction()
        {
            Tile actionTile = _board[_actionCoords];
            switch (_actionType)
            {
                case Utility.PlayerAction.Flag:
                    _board.ToggleFlag(_actionCoords);
                    // Toggle flag: .Remove and .Add automatically perform lookup
                    if (!_flagCoords.Remove(_actionCoords)) _flagCoords.Add(_actionCoords);
                    break;

                case Utility.PlayerAction.Reveal:
                    // Transition to next game state if idling TODO: make more elegant
                    if (State == GameState.Idling)
                        _board.GenerateMines(_actionCoords);
                        State = GameState.Playing;

                    if (!_flagCoords.Contains(_actionCoords) && actionTile.IsMine)
                    {
                        Playing = false;
                        State = GameState.Exploded;
                    }
                    else _board.Flood(_actionCoords);
                    
                    break;

                case Utility.PlayerAction.Chord:
                    // Think about ways to make more elegant
                    if (actionTile.AdjacentFlags > actionTile.AdjacentMines)
                        _board.Chord(_actionCoords);

                    if (_board.CountAdjacent(_actionCoords, Tile.TileState.Exploded) != 0)
                        State = GameState.Exploded;

                    break;
            }
        }
    
        // Determines whether game can continue
        public void GameStatusCheck()
        {
            if (State == GameState.Exploded) Playing = false;
                _board.ExplodeMines();

            if (State == GameState.Completed) Playing = false;
        }
    }
}
