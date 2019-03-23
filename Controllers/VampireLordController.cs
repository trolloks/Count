﻿using System;
using Count.Enums;
using Count.Models;
using Count.Utils;

namespace Count.Controllers
{
    public class VampireLordController
    {
        private const int BASE_FEED_DC = 8;
        private const int BASE_CHECK_ROLL = 20;

        private Models.Game _game;
        private VampireLord _vampireLord { get; set; }

        private bool _firstFeed = true;

        /// <summary>
        /// Hunger logic
        /// </summary>
        public static int BLOOD_WARNING_THRESHOLD = 5;

        public VampireLordController(Models.Game game)
        {
            _game = game;

            // Create Vampire Lord
            _vampireLord = new VampireLord()
            {
                Hitpoints = 20,
                ActionPointsMax = 1,
                Blood = 5,
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
            feedCheck = _firstFeed || feedRoll >= BASE_FEED_DC;

            if (feedCheck)
            {
                status = FeedStatus.FED;

                // Kill Villager
                var hasVillagers = village.TryKillVillager();
                if (!hasVillagers)
                    return FeedStatus.FAILED;

                // Could convert into a vampire
                var follower = _game.Castle.CreateVampire(_firstFeed);
                if (follower != null)
                    status = FeedStatus.CONVERTED;

                // Get Blood
                IncreaseBlood(3);
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
        /// Current Blood
        /// </summary>
        public int Blood
        {
            get { return _vampireLord.Blood; }
        }

        /// <summary>
        /// Current Corpses
        /// </summary>
        public int Corpses
        {
            get { return _vampireLord.Corpses; }
        }

        #endregion

        #region "General Actions"

        /// <summary>
        /// Kills you definitely
        /// </summary>
        public void ForceKill()
        {
            _vampireLord.Hitpoints = 0;
        }

        public void SpendBlood(int i)
        {
            _vampireLord.Blood -= i;
        }

        public void IncreaseBlood(int i)
        {
            _vampireLord.Blood += i;
        }

        public void SpendCorpses(int i)
        {
            _vampireLord.Corpses -= i;
        }

        public void IncreaseCorpses(int i)
        {
            _vampireLord.Corpses += i;
        }

        /// <summary>
        /// Pay X Amount of blood because of blood curse
        /// </summary>
        public void PayBlood()
        {
            _vampireLord.Blood = Math.Max(0, Blood - 1);
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
