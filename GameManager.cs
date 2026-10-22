namespace mnswpr
{
    class GameManager(int boardWidth, int boardHeight, int mines)
    { 
        public bool Playing { get; private set; } = false;
        private int _actionX, _actionY;
        private Utility.PlayerAction actionType;

        private readonly int _boardWidth = boardWidth;
        private readonly int _boardHeight = boardHeight;
        private readonly int _mines = mines;
        private readonly Board _board = new(boardWidth, boardHeight, mines);

        private static (Utility.PlayerAction action, Utility.Coordinates) GetAction(int boardWidth, int boardHeight)
        {
            Console.Write("\nAction:\n> ");
            string? inputAction = Console.ReadLine();

            while (string.IsNullOrEmpty(inputAction) || !Utility.IsActionValid(inputAction, boardWidth, boardHeight))
            {
                Console.Write("\nInvalid input, please try again:\n> ");
                inputAction = Console.ReadLine();
            }

            string[] inputParams = inputAction.Split();

            return (Utility.ACTION_CHAR_TO_ENUM[Convert.ToChar(inputParams[0])], new(Convert.ToInt32(inputParams[1]), Convert.ToInt32(inputParams[2])));
        }


        // Handles action side-effects and returns whether game can continue
        private static void HandleAction(int actType, int actRow, int actCol, Board board)
        {
            switch (actType)
            {
                
            }
        }
    }
}
