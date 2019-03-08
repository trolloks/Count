using System;
using Count.Enums;
using Count.Models;
using Count.Utils;

namespace Count.Controllers
{
    public class VampireLordController
    {
        private const int BASE_FEED_DC = 5;
        private const int BASE_CHECK_ROLL = 20;

        private Models.Game _game;
        private VampireLord VampireLord { get; set; }

        private bool _firstFeed = true;

        /// <summary>
        /// Hunger logic
        /// </summary>
        public static int HUNGER_WARNING_THRESHOLD = 5;
        public static int HUNGER_STARVING_THRESHOLD = 10;

        public VampireLordController(Models.Game game)
        {
            _game = game;

            // Create Vampire Lord
            VampireLord = new VampireLord();
            VampireLord.Hitpoints = 20;
            VampireLord.ActionPointsMax = 1;
            VampireLord.LastFed = _game.World.Day;
            VampireLord.WorldLocation = _game.StartingWorldLocation;
            VampireLord.RegionLocation = _game.StartingRegionLocation;

            Sleep();
        }


        #region "Night Actions"
        /// <summary>
        ///  Checks if you succeed on feeding on a unsuspecting villager
        /// </summary>
        public FeedStatus? Feed()
        {
            var status = FeedStatus.FAILED;
            var locationObject = _game.World.GetRegion(WorldLocation).GetLocationObjectAtLocation(RegionLocation);
            if (locationObject == null || locationObject.GetType() != typeof(VillageController))
                return null;

            var village = locationObject as VillageController;

            var feedCheck = true;
            var feedRoll = Randomizer.Instance.Roll(1, BASE_CHECK_ROLL);
            if (GameViewController.IS_DEV)
            {
                Console.WriteLine($"(DEV) FEED CHECK: {feedRoll}");
                Console.WriteLine($"(DEV) FEED DC CHECK: {(BASE_FEED_DC + Math.Round((BASE_CHECK_ROLL - BASE_FEED_DC) * village.Suspicion))}");
            }

            feedCheck = _firstFeed || feedRoll >= (BASE_FEED_DC + Math.Round((BASE_CHECK_ROLL - BASE_FEED_DC) * village.Suspicion));

            if (feedCheck)
            {
                status = FeedStatus.FED;
                // Effects on you
                VampireLord.LastFed = _game.World.Day;
                // Could convert into a vampire
                var follower = _game.Castle.CreateVampire(_firstFeed);
                if (follower != null)
                    status = FeedStatus.CONVERTED;

                // Kill Villager
                village.KillVillager();
                // Get Soul
                IncreaseSouls(1);
            }

            _firstFeed = false;
            return status;
        }



        #endregion

        #region "Day Actions"
        /// <summary>
        /// Rest and regain actionpoints and ---possibly hitpoints?
        /// </summary>
        public void Sleep()
        {
            VampireLord.WorldLocation = _game.Castle.WorldLocation;
            VampireLord.RegionLocation = _game.Castle.RegionLocation;
            VampireLord.ActionPoints = VampireLord.ActionPointsMax;
        }
        #endregion

        #region "Properties"
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
        public Location WorldLocation
        {
            get { return VampireLord.WorldLocation; }
        }

        public Location RegionLocation
        {
            get { return VampireLord.RegionLocation; }
        }

        /// <summary>
        /// Your current actionpoints
        /// </summary>
        public int ActionPoints
        {
            get { return VampireLord.ActionPoints; }
        }
        #endregion
        /// <summary>
        /// The level of your hunger
        /// </summary>
        public long DetermineHungerLevel()
        {
            return _game.World.Day - VampireLord.LastFed;
        }

        /// <summary>
        /// Tries to kill you
        /// </summary>
        /*public bool TryKill()
        {
            bool couldKill = !Followers.Any(i => i.Follower.GetType() == typeof(Zombie)); // Only zombies can block death
            if (couldKill)
            {
                _followers.Remove(Followers.FirstOrDefault(i => i.Follower.GetType() == typeof(Zombie)));
                ForceKill();
            }
            return couldKill;
        }*/

        /// <summary>
        /// Kills you definitely
        /// </summary>
        public void ForceKill()
        {
            VampireLord.Hitpoints = 0;
        }

        public void SpendSouls(int i)
        {
            VampireLord.Souls -= i;
        }

        public void IncreaseSouls(int i)
        {
            VampireLord.Souls += i;
        }


        /// <summary>
        /// Damages you
        /// </summary>
        /// <param name="i">amount of damage</param>
        public void Damage(int i)
        {
            VampireLord.Hitpoints -= i;
        }

        /// <summary>
        /// Current Souls
        /// </summary>
        public int Souls
        {
            get { return VampireLord.Souls; }
        }

        /// <summary>
        /// Exerts your actionpoints
        /// </summary>
        /// <param name="i">amount of exertion</param>
        public bool TryExert(int i)
        {
            if (i <= VampireLord.ActionPoints)
            {
                VampireLord.ActionPoints -= i;
                return true;
            }
            return false;
        }

        public void Move(Location worldLocation, Location regionLocation)
        {
            VampireLord.WorldLocation = worldLocation;
            VampireLord.RegionLocation = regionLocation;
        }
    }
}
