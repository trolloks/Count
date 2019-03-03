using System;
using Count.Enums;
using Count.Models;
using Count.Models.Followers;
using Count.Utils;

namespace Count.Controllers
{
    public class VampireController : FollowerController
    {
        private const int BASE_FEED_DC = 5;
        private const int BASE_CHECK_ROLL = 20;

        public VampireController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
            _follower = new Vampire() { Available = true, WorldLocation = worldLocation, RegionLocation = regionLocation };
        }

        public void MoveToVillage(VillageController village)
        {
            _follower.WorldLocation = village.WorldLocation;
            _follower.RegionLocation = village.RegionLocation;
        }

        /// <summary>
        ///  Checks if you succeed on feeding on a unsuspecting villager
        /// </summary>
        public FeedStatus? Feed(WorldController worldController, VampireLordController vampireLord)
        {
            var status = FeedStatus.FAILED;
            var locationObject = worldController.GetRegion(_follower.WorldLocation).GetLocationObjectAtLocation(_follower.RegionLocation);
            if (locationObject == null || locationObject.GetType() != typeof(VillageController))
                return null;

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
                status = FeedStatus.FED;
                // Kill Villager
                village.KillVillager();
                // Get Soul
                vampireLord.IncreaseSouls(1);
            }

            return status;
        }
    }
}
