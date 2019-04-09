using System;
using System.Linq;
using Count.Models;
using Count.Utils;

namespace Count.Controllers
{
    public abstract class HeroController : CreatureController
    {
        protected Hero _hero { get { return _object as Hero; } }
        public Hero Hero { get { return _hero; } }
        
        private static int BASE_SPAWN_MIN_DAY = 2;
        private static int BASE_SPAWN_DC = 8;
        private static int BASE_CHECK_ROLL = 20;

        protected HeroController(Location worldLocation) : base(worldLocation) { }


        /// <summary>
        ///  Checks if you succeed on creating a follower
        /// </summary>
        public static HeroController TryCreateHero(Type heroType, Game game, Location worldLocation, bool force = false)
        {
            // Roll for adventurer spawn
            // - MAX Game.HERO_MAX
            // - MUST ROLL MORE THAN BASE_SPAWN_DC
            // - only able to spawn after day BASE_SPAWN_MIN_DAY
            // - have 'pity' counter
            if (game.Heroes.Count < Game.HERO_MAX)
            {
                var roll = Randomizer.Instance.Roll(1, BASE_CHECK_ROLL);
                var convertCheck = (roll >= BASE_SPAWN_DC && game.World.Day >= BASE_SPAWN_MIN_DAY) || force;
                if (convertCheck)
                {
                    if (heroType == typeof(Fighter))
                        return new FighterController(worldLocation) as HeroController;
                }
            }

            return null;
        }

        public virtual bool Adventure(Models.Game game)
        {
            return false;
        }
    }
}
