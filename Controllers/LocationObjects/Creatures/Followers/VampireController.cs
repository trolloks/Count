using System;
using Count.Enums;
using Count.Models;
using Count.Models.Followers;
using Count.Utils;

namespace Count.Controllers
{
    public class VampireController : FollowerController
    {
        private const int BASE_FEED_DC = 0;
        private const int BASE_CHECK_ROLL = 20;

        public VampireController(Location worldLocation) : base(worldLocation)
        {
            _object = new Vampire() { WorldLocation = worldLocation, Damage = 3, Hitpoints = 3, DefenceRating = 12};
        }

        /// <summary>
        ///  Checks if you succeed on feeding on a unsuspecting villager
        /// </summary>
        public FeedStatus? Feed(Game game, VampireLordController vampireLord)
        {
            var status = FeedStatus.FAILED;
            var locationObject = game.World.GetLocationObjectAtLocation(_object.WorldLocation);
            if (locationObject == null || locationObject.GetType() != typeof(VillageController))
                return null;

            var village = locationObject as VillageController;
            foreach (var hero in game.Heroes)
            {
                if (LocationUtil.CompareLocations(hero.WorldLocation, village.WorldLocation))
                    return FeedStatus.BLOCKED;
            }

            var feedCheck = true;
            var feedRoll = Randomizer.Instance.Roll(1, BASE_CHECK_ROLL);
            feedCheck = feedRoll >= BASE_FEED_DC;

            if (feedCheck)
            {
                status = FeedStatus.FED;
                // Get Blood
                vampireLord.IncreaseBlood(1);
            }

            return status;
        }
    }
}
