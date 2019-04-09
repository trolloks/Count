using System;
using Count.Models;
using Count.Models.Followers;
using Count.Utils;

namespace Count.Controllers
{
    public abstract class FollowerController : CreatureController
    {
        protected Follower _follower { get { return _object as Follower; } }

        private const int BASE_CONVERT_DC = 5;
        private const int BASE_CHECK_ROLL = 20;

        protected FollowerController(Location worldLocation): base(worldLocation) { }

        /// <summary>
        ///  Checks if you succeed on creating a follower
        /// </summary>
        public static FollowerController TryCreateFollower(Type followerType, Location worldLocation, bool force)
        {
            var convertCheck = true;
            var convertRoll = Randomizer.Instance.Roll(1, BASE_CHECK_ROLL);
            if (GameViewController.IS_DEV)
            {
                Console.WriteLine($"(DEV) CONVERT CHECK: {convertRoll}");
                Console.WriteLine($"(DEV) CONVERT DC CHECK: {BASE_CONVERT_DC}");
            }

            convertCheck = force || convertRoll >= (BASE_CONVERT_DC);

            if (convertCheck)
            {
                if (followerType == typeof(Zombie))
                    return new ZombieController(worldLocation) as FollowerController;
                if (followerType == typeof(Vampire))
                    return new VampireController(worldLocation) as FollowerController;
            }

            return null;
        }

        public Follower Follower { get { return _follower; } }
    }
}
