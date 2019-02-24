using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Count.Models;
using Count.Models.Followers;
using Count.Utils;

namespace Count.Controllers
{
    public class VampireLordController
    {
        private const int BASE_FEED_DC = 8;
        private const int BASE_CONVERT_DC = 5;
        private const int BASE_CHECK_ROLL = 20;

        private WorldController _worldController;
        private VampireLord VampireLord { get; set; }
        private CastleController _castleController { get; set; } 
        private List<FollowerController> _followers { get; set; }

        /// <summary>
        /// Hunger logic
        /// </summary>
        public static int HUNGER_WARNING_THRESHOLD = 5;
        public static int HUNGER_STARVING_THRESHOLD = 10;

        public VampireLordController(WorldController worldController, CastleController castleController, Location startingWorldLocation, Location startingRegionLocation)
        {
            _worldController = worldController;
            _castleController = castleController;
            _followers = new List<FollowerController>();

            // Create Vampire Lord
            VampireLord = new VampireLord();
            VampireLord.Hitpoints = 50;
            VampireLord.ActionPointsMax = 1;
            VampireLord.LastFed = _worldController.Day;
            VampireLord.WorldLocation = startingWorldLocation;
            VampireLord.RegionLocation = startingRegionLocation;
          
            Sleep();
        }

        #region "Night Actions"
        /// <summary>
        ///  Checks if you succeed on feeding on a unsuspecting villager
        /// </summary>
        public bool Feed()
        {
            var locationObject = _worldController.GetRegion(WorldLocation).GetLocationObjectAtLocation(RegionLocation);
            if (locationObject == null || locationObject.GetType() != typeof(VillageController))
                return false;

            var village = locationObject as VillageController;

            var feedCheck = true;
            var feedRoll = Randomizer.Instance.Roll(1, BASE_CHECK_ROLL);
            if (Game.IS_DEV)
            {
                Console.WriteLine($"(DEV) FEED CHECK: {feedRoll}");
                Console.WriteLine($"(DEV) FEED DC CHECK: {(BASE_FEED_DC + Math.Round((BASE_CHECK_ROLL - BASE_FEED_DC) * village.Suspicion))}");
            }

            feedCheck = feedRoll >= (BASE_FEED_DC + Math.Round((BASE_CHECK_ROLL - BASE_FEED_DC) * village.Suspicion));

            if (feedCheck)
            {
                // Effects on you
                VampireLord.LastFed = _worldController.Day;
            }

            return feedCheck;
        }

        /// <summary>
        ///  Checks if you succeed on converting a villager
        /// </summary>
        public Type TryConvertFollower()
        {
            var locationObject = _worldController.GetRegion(WorldLocation).GetLocationObjectAtLocation(RegionLocation);
            if (locationObject == null || locationObject.GetType() != typeof(VillageController))
                return null;

            var village = locationObject as VillageController;

            var convertCheck = true;
            var convertRoll = Randomizer.Instance.Roll(1, BASE_CHECK_ROLL);
            if (Game.IS_DEV)
            {
                Console.WriteLine($"(DEV) CONVERT CHECK: {convertRoll}");
                Console.WriteLine($"(DEV) CONVERT DC CHECK: {(BASE_CONVERT_DC + Math.Round((BASE_CHECK_ROLL - BASE_CONVERT_DC) * village.Suspicion))}");
            }

            convertCheck = convertRoll >= (BASE_CONVERT_DC + Math.Round((BASE_CHECK_ROLL - BASE_CONVERT_DC) * village.Suspicion));

            if (convertCheck)
            {
                var followerController = village.ConvertVillagerToFollower();
                if (followerController != null)
                {
                    _followers.Add(followerController);
                    return followerController.Follower.GetType();
                }
            }

            return null;
        }
        #endregion

        #region "Day Actions"
        /// <summary>
        /// Rest and regain actionpoints and ---possibly hitpoints?
        /// </summary>
        public void Sleep()
        {
            VampireLord.WorldLocation = _castleController.WorldLocation;
            VampireLord.RegionLocation = _castleController.RegionLocation;
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
            return _worldController.Day - VampireLord.LastFed;
        }

        public ReadOnlyCollection<FollowerController> Followers
        {
            get { return _followers.AsReadOnly(); }
        }

        /// <summary>
        /// Tries to kill you
        /// </summary>
        public bool TryKill()
        {
            bool couldKill = !Followers.Any(i => i.Follower.GetType() == typeof(Zombie)); // Only zombies can block death
            if (couldKill)
            {
                _followers.Remove(Followers.FirstOrDefault(i => i.Follower.GetType() == typeof(Zombie)));
                ForceKill();
            }
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
        /// <param name="i">amount of damage</param>
        public void Damage(int i)
        {
            VampireLord.Hitpoints -= i;
        }

        /// <summary>
        /// Exerts your actionpoints
        /// </summary>
        /// <param name="i">amount of exertion</param>
        public void Exert(int i)
        {
            VampireLord.ActionPoints -= i;
        }

        public void Move(Location worldLocation, Location regionLocation)
        {
            VampireLord.WorldLocation = worldLocation;
            VampireLord.RegionLocation = regionLocation;
        }

            
    }
}
