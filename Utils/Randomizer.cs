using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count.Utils
{
    public static class Randomizer
    {
        public static int Roll(int iterations, int range, Random random)
        {
            int roll = 0;
            for(int i = 0; i < iterations; i++)
            {
                roll += (random.Next(range) + 1);
            }
            return roll;
        }
    }
}
