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
        private VampireLord _vampireLord { get; set; }

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
            _vampireLord = new VampireLord()
            {
                Hitpoints = 20,
                ActionPointsMax = 1,
                LastFed = _game.World.Day,
                WorldLocation = _game.StartingWorldLocation,
                RegionLocation = _game.StartingRegionLocation
            };
          
            // Start first night after a sleep
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
                _vampireLord.LastFed = _game.World.Day;
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
            _vampireLord.WorldLocation = _game.Castle.WorldLocation;
            _vampireLord.RegionLocation = _game.Castle.RegionLocation;
            _vampireLord.ActionPoints = _vampireLord.ActionPointsMax;
        }
        #endregion

        #region "Properties"
        /// <summary>
        ///  Checks if you are dead
        /// </summary>
        public bool IsDead
        {
            get { return _vampireLord.Hitpoints <= 0; }
        }

        /// <summary>
        /// Your current hitpoints
        /// </summary>
        public int Hitpoints
        {
            get { return _vampireLord.Hitpoints; }
        }
        /// <summary>
        /// Your current location
        /// </summary>
        public Location WorldLocation
        {
            get { return _vampireLord.WorldLocation; }
        }

        public Location RegionLocation
        {
            get { return _vampireLord.RegionLocation; }
        }

        /// <summary>
        /// Your current actionpoints
        /// </summary>
        public int ActionPoints
        {
            get { return _vampireLord.ActionPoints; }
        }

        /// <summary>
        /// Current Souls
        /// </summary>
        public int Souls
        {
            get { return _vampireLord.Souls; }
        }

        #endregion

        #region "General Actions"

        /// <summary>
        /// The level of your hunger
        /// </summary>
        public long DetermineHungerLevel()
        {
            return _game.World.Day - _vampireLord.LastFed;
        }

        /// <summary>
        /// Kills you definitely
        /// </summary>
        public void ForceKill()
        {
            _vampireLord.Hitpoints = 0;
        }

        public void SpendSouls(int i)
        {
            _vampireLord.Souls -= i;
        }

        public void IncreaseSouls(int i)
        {
            _vampireLord.Souls += i;
        }

        /// <summary>
        /// Damages you
        /// </summary>
        /// <param name="i">amount of damage</param>
        public void Damage(int i)
        {
            _vampireLord.Hitpoints -= i;
        }

        /// <summary>
        /// Exerts your actionpoints
        /// </summary>
        /// <param name="i">amount of exertion</param>
        public bool TryExert(int i)
        {
            if (i <= _vampireLord.ActionPoints)
            {
                _vampireLord.ActionPoints -= i;
                return true;
            }
            return false;
        }

        public void MoveLocation(Location worldLocation, Location regionLocation)
        {
            _vampireLord.WorldLocation = worldLocation;
            _vampireLord.RegionLocation = regionLocation;
        }

        #endregion
    }
}
