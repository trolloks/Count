using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count.Utils;

namespace Count.Models
{
    public class VampireLord
    {
        private const int BASE_FEED_DC = 10;
        private const int BASE_CHECK_ROLL = 20;
        private Random _random;

        public VampireLord()
        {
            _random = new Random();
        }


        public int Hitpoints { get; set; }
        /// <summary>
        /// Last day the vampire was fed
        /// </summary>
        public long LastFed { get; set; }

        /// <summary>
        /// Villagers converted into followers
        /// </summary>
        public int Followers { get; set; }
        /// <summary>
        /// Vampire's hiding spot
        /// </summary>
        public Location Location { get; set; }

        public bool Feed(Village village)
        {
            var feedRoll = Randomizer.Roll(1, BASE_CHECK_ROLL, _random);
            if (Game.IS_DEV)
            {
                Console.WriteLine($"(DEV) FEED CHECK: {feedRoll}");
                Console.WriteLine($"(DEV) FEED DC CHECK: {(BASE_FEED_DC + Math.Round(((float)BASE_FEED_DC) * village.Suspicion))}");
            }

            return feedRoll >= (BASE_FEED_DC + Math.Round(((float)BASE_FEED_DC) * village.Suspicion));
        }
    }
}
