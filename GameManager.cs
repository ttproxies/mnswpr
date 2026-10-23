namespace mnswpr
{
    class GameManager
    { 
        // Properties
        public bool Playing { get; private set; } = true;
        public GameState State { get; private set; } = GameState.Idling;
        
        // Fields
        private readonly List<Utility.Coordinates> _flagCoords = [];
        private Utility.Coordinates _actionCoords;
        private Utility.PlayerAction _actionType;

        // Constructor
        private readonly int _boardWidth;
        private readonly int _boardHeight;
        private readonly int _mines;
        private readonly Board _board;

        public GameManager(int boardWidth, int boardHeight, int mines)
        {
            (_boardWidth, _boardHeight, _mines) = (boardWidth, boardHeight, mines);
            _board = new(this, boardWidth, boardHeight, mines);   
        }

        public enum GameState
        {
            Idling, // Player has not yet revealed any tiles
            Playing, // Round in progress
            Exploded, // Player has exploded
            Completed // Player has won
        }

        // TODO: make more elegant
        public void DisplayBoard() => _board.DisplayBoard();

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

            (_actionType, _actionCoords) = (Utility.ACTION_CHAR_TO_ENUM[Convert.ToChar(inputParams[0])], new(Convert.ToInt32(inputParams[1]), Convert.ToInt32(inputParams[2])));
        }
    
        public void HandleAction()
        {
            Tile actionTile = _board[_actionCoords];
            switch (_actionType)
            {
                case Utility.PlayerAction.Flag:
                    _board.ToggleFlag(_actionCoords);
                    _flagCoords.Add(_actionCoords);
                    break;

                case Utility.PlayerAction.Reveal:
                    // Transition to next game state if idling TODO: make more elegant
                    if (State == GameState.Idling)
                        _board.GenerateMines(_actionCoords);
                        State = GameState.Playing;

                    if (actionTile.IsMine)
                    {
                        State = GameState.Exploded;
                        Playing = false;
                    }

                    _board.Flood(_actionCoords);
                    break;

                case Utility.PlayerAction.Chord:
                    if (actionTile.AdjacentFlags == actionTile.AdjacentMines)
                        _board.Chord(_actionCoords);
                    break;
            }
        }
    }
}
