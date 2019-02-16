using System;
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

        /// <summary>
        /// Hunger logic
        /// </summary>
        public static int HUNGER_WARNING_THRESHOLD = 5;
        public static int HUNGER_STARVING_THRESHOLD = 10;

        public VampireLordController(WorldController worldController)
        {
            _worldController = worldController;

            // Create Vampire Lord
            VampireLord = new VampireLord();
            VampireLord.Hitpoints = 50;
            VampireLord.ActionPointsMax = 1;
            VampireLord.LastFed = _worldController.Day;

            // Init
            MoveLocation();
            Sleep();
        }

        #region "Night Actions"
        /// <summary>
        ///  Checks if you succeed on feeding on a unsuspecting villager
        /// </summary>
        public bool Feed()
        {
            var feedCheck = true;
            var feedRoll = Randomizer.Instance.Roll(1, BASE_CHECK_ROLL);
            if (Game.IS_DEV)
            {
                Console.WriteLine($"(DEV) FEED CHECK: {feedRoll}");
                Console.WriteLine($"(DEV) FEED DC CHECK: {(BASE_FEED_DC + Math.Round((BASE_CHECK_ROLL - BASE_FEED_DC) * _worldController.GetCurrentVillage().Suspicion))}");
            }

            feedCheck = feedRoll >= (BASE_FEED_DC + Math.Round((BASE_CHECK_ROLL - BASE_FEED_DC) * _worldController.GetCurrentVillage().Suspicion));

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
        public Follower ConvertFollower(Villager villager)
        {
            var convertCheck = true;
            var convertRoll = Randomizer.Instance.Roll(1, BASE_CHECK_ROLL);
            if (Game.IS_DEV)
            {
                Console.WriteLine($"(DEV) CONVERT CHECK: {convertRoll}");
                Console.WriteLine($"(DEV) CONVERT DC CHECK: {(BASE_CONVERT_DC + Math.Round((BASE_CHECK_ROLL - BASE_CONVERT_DC) * _worldController.GetCurrentVillage().Suspicion))}");

                Console.WriteLine($"(DEV) VILLAGER STR: {villager.Strength}");
                Console.WriteLine($"(DEV) VILLAGER INT: {villager.Intelligence}");
                Console.WriteLine("");
                Console.WriteLine($"(DEV) ZOMBIE CHANCE: {(float)villager.Strength / (float)(villager.Intelligence + villager.Strength) * 100}");
                Console.WriteLine($"(DEV) VAMPIRE CHANCE: {(float)villager.Intelligence / (float)(villager.Intelligence + villager.Strength) * 100}");
            }

            convertCheck = convertRoll >= (BASE_CONVERT_DC + Math.Round((BASE_CHECK_ROLL - BASE_CONVERT_DC) * _worldController.GetCurrentVillage().Suspicion));

            if (convertCheck)
            {
                Follower follower = null;
                var rollChance = Randomizer.Instance.Roll(1, villager.Intelligence + villager.Strength);
                if (rollChance <= villager.Intelligence)
                {
                    follower = new Vampire { PreviousLife = villager };
                    VampireLord.Followers.Add(follower);
                }
                else
                {
                    follower = new Zombie { PreviousLife = villager };
                    VampireLord.Followers.Add(follower);
                }

                return follower;
            }

            return null;
        }

        /// <summary>
        /// Move your hiding location
        /// </summary>
        public void MoveLocation()
        {
            VampireLord.Location = _worldController.GenerateWorldLocation();
        }
        #endregion

        #region "Day Actions"
        /// <summary>
        /// Rest and regain actionpoints and ---possibly hitpoints?
        /// </summary>
        public void Sleep()
        {
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
        public Location Location
        {
            get { return VampireLord.Location; }
        }

        public ReadOnlyCollection<Follower> Followers
        {
            get { return VampireLord.Followers.AsReadOnly(); }
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

        /// <summary>
        /// Tries to kill you
        /// </summary>
        public bool TryKill()
        {
            bool couldKill = !Followers.Any(i => i.GetType() == typeof(Zombie)); // Only zombies can block death
            if (!couldKill)
                KillFollower(typeof(Zombie));
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

        public void KillFollower(Type followerType)
        {
            var collection = VampireLord.Followers;
            if (followerType != null)
            {
                collection = collection.Where(i => i.GetType() == followerType).ToList();
            }
            var unluckyFollowerIndex = Randomizer.Instance.Random.Next(Followers.Count);
            var unluckyFollower = collection[unluckyFollowerIndex];
            // remove
            VampireLord.Followers.Remove(unluckyFollower);
        }
    }
}
