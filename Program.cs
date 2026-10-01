using System;

namespace Game
{
    class Game
    {
        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine("Usage: dotnet run <w> <h>");
            }

            var board = new Board(args[0], args[1], args[2]);
        }
    }

    class Board
    {
        public int w, h, mines;
        public Board(int w, int h, int mines)
        {
            w = w;
            h = h;
            mines = mines;
        }


    }
}