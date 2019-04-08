using System;
using System.Linq;
using Count.Models;

namespace Count.Controllers
{
    public class FighterController : HeroController
    {
        protected static int _globalFighterCount;
        public FighterController(Location worldLocation) : base(worldLocation)
        {
            _object = new Fighter()
            {
                Name = $"Fighter-{++_globalFighterCount}", // temp
                Hitpoints = 3,
                Damage = 3,
                DefenceRating = 15,
                WorldLocation = worldLocation
            };
        }

        public override bool Adventure(Game game)
        {
            return base.Adventure(game);
        }
    }
}
