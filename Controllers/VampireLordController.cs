using System;
using Count.Models;
using Count.Utils;

namespace Count.Controllers
{
    public class VampireLordController
    {
        private const int BASE_FEED_DC = 10;
        private const int BASE_CHECK_ROLL = 20;

        private WorldController _worldController;
        private VampireLord VampireLord { get; set; }

        public VampireLordController(WorldController worldController)
        {
            _worldController = worldController;

            // Create Vampire Lord
            VampireLord = new VampireLord();
            VampireLord.Hitpoints = 10;
            VampireLord.ActionPoints = 5;

            // Init
            Feed(1, true);
            MoveLocation();
        }

        /// <summary>
        ///  Checks if you succeed on feeding on a unsuspecting villager
        /// </summary>
        public bool Feed(long day)
        {
            return Feed(day, false);
        }

        private bool Feed(long day, bool forceSuccess)
        {
            var feedCheck = true;
            if (!forceSuccess)
            {
                var feedRoll = Randomizer.Instance.Roll(1, BASE_CHECK_ROLL);
                if (Game.IS_DEV)
                {
                    Console.WriteLine($"(DEV) FEED CHECK: {feedRoll}");
                    Console.WriteLine($"(DEV) FEED DC CHECK: {(BASE_FEED_DC + Math.Round((BASE_FEED_DC) * _worldController.GetCurrentVillage().Suspicion))}");
                }

                feedCheck = feedRoll >= (BASE_FEED_DC + Math.Round((BASE_FEED_DC) * _worldController.GetCurrentVillage().Suspicion));
            }
            
            if (feedCheck)
            {
                // Effects on you
                VampireLord.LastFed = day;
                if (!forceSuccess)
                    VampireLord.Followers++;
            }

            return feedCheck;
        }


        /// <summary>
        ///  Checks if you are dead
        /// </summary>
        public bool IsDead
        {
            get { return VampireLord.Hitpoints <= 0; }
        }

        /// <summary>
        /// Your current hitpoints
        /// </summary>
        public int Hitpoints
        {
            get { return VampireLord.Hitpoints; }
        }

        /// <summary>
        /// Your current location
        /// </summary>
        public Location Location
        {
            get { return VampireLord.Location; }
        }

        public int Followers
        {
            get { return VampireLord.Followers; }
        }

        /// <summary>
        /// Hunger logic
        /// </summary>
        public static int HUNGER_WARNING_THRESHOLD = 5;
        public static int HUNGER_STARVING_THRESHOLD = 10;

        /// <summary>
        /// The level of your hunger
        /// </summary>
        public long DetermineHungerLevel(long day)
        {
            return day - VampireLord.LastFed;
        }

        /// <summary>
        /// Tries to kill you
        /// </summary>
        public bool TryKill()
        {
            bool couldKill = Followers <= 0;
            if (!couldKill)
                VampireLord.Followers--;
            else
                ForceKill();
            return couldKill;
        }

        /// <summary>
        /// Kills you definitely
        /// </summary>
        public void ForceKill()
        {
            VampireLord.Hitpoints = 0;
        }

        /// <summary>
        /// Damages you
        /// </summary>
        public void Damage(int i)
        {
            VampireLord.Hitpoints -= i;
        }

        /// <summary>
        /// Move your hiding location
        /// </summary>
        public void MoveLocation()
        {
            VampireLord.Location = _worldController.GenerateWorldLocation();
        }


        /// <summary>
        /// Checks to see if you are hiding at current location
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool IsHidingAt(Location location)
        {
            return location.X == VampireLord.Location.X && location.Y == VampireLord.Location.Y;
        }

        public int ActionPoints
        {
            get { return VampireLord.ActionPoints; }
        }
    }
}
