using System;

namespace Count
{
    class Program
    {
        private static GameViewController _game;

        static void Main(string[] args)
        {
            _game = new GameViewController();
            _game.Start();
        }
    }
}
