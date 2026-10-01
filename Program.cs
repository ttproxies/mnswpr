using System;

namespace Game
{
    class Game
    {
        public static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine("Usage: dotnet run <w> <h> <mines>");
                return 1;
            }

            int w = int.Parse(args[0]);
            int h = int.Parse(args[1]);
            int mines =  int.Parse(args[2]); 

            var board = new Board(w, h, mines);
            board.DisplayBoard();
            
            return 0;
        }
    }

    class Board
    {
        public int w, h, mines;
        public Board(int w, int h, int mines)
        {
            this.w = w;
            this.h = h;
            this.mines = mines;
        }

        public void DisplayBoard()
        {
            for (int i = 0; i < this.h; i++)
            {
                for (int j = 0; j < this.w; j++)
                {
                    Console.Write("# ");
                }
                Console.WriteLine();
            }
        }
    }
}