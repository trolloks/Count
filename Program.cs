using System;

namespace Count
{
    class Program
    {
        private static Game _game;

        static void Main(string[] args)
        {
            _game = new Game();
            _game.Start();
        }
    }
}
