using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count.Utils
{
    public class Randomizer
    {
        private Random _random;
        private static Randomizer _randomizer;

        private Randomizer()
        {
            _random = new Random();
        }

        public static Randomizer Instance
        {
            get
            {
                if (_randomizer != null)
                    _randomizer = new Randomizer();

                return _randomizer;
            }
        }

        public Random Random
        {
            get { return _random; }
        }

        public int Roll(int iterations, int range)
        {
            int roll = 0;
            for(int i = 0; i < iterations; i++)
            {
                roll += (_random.Next(range) + 1);
            }
            return roll;
        }
    }
}
