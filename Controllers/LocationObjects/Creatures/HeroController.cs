using Count.Models;
using Count.Utils;

namespace Count.Controllers
{
    public abstract class HeroController<T> : CreatureController<T> where T : Hero
    {
        private static int BASE_SPAWN_MAX_HERO = 5;
        private static int BASE_SPAWN_MIN_DAY = 5;
        private static int BASE_SPAWN_DC = 16;
        private static int BASE_CHECK_ROLL = 20;
        private static int BASE_SPAWN_PITY = 5;

        protected HeroController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation) { }


        /// <summary>
        ///  Checks if you succeed on creating a follower
        /// </summary>
        public static HeroController<T> TryCreateHero(Game game, Location worldLocation, Location regionLocation, bool force)
        {
            // Roll for adventurer spawn
            // - MAX 5
            // - MUST ROLL MORE THAN 16
            // - IF SUCCEEDS CAN ROLL AGAIN
            // - only able to spawn after day 5
            // - have 'pity' counter
            var roll = Randomizer.Instance.Roll(1, BASE_CHECK_ROLL);
            var convertCheck = (roll >= BASE_SPAWN_DC && game.World.Day >= BASE_SPAWN_MIN_DAY) || force;
            if (convertCheck)
            {
                if (typeof(T) == typeof(Fighter))
                    return new FighterController(worldLocation, regionLocation) as HeroController<T>;
            }

            return null;
        }

        public abstract bool Hero(Models.Game game);
    }
}
