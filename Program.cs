using System;

namespace Count
{
    class Program
    {
        private static Game _game;

        static void Main(string[] args)
        {
            // Initialise menu
            ShowMenu();
        }

        private static void ShowMenu()
        {
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("THE COUNT");
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("1. Start New Game");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("(Any other command will exit the game)");
            Console.Write(": ");

            var menuResult = Console.ReadLine();
            if (menuResult == "1")
            {
                _game = new Game();
                _game.Start();
            }
        }
    }
}
