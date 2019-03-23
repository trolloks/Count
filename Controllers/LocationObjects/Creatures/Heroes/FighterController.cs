using System;
using System.Linq;
using Count.Models;

namespace Count.Controllers
{
    public class FighterController : HeroController
    {
        protected static int _globalFighterCount;
        public FighterController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
            _object = new Fighter()
            {
                Name = $"Fighter-{++_globalFighterCount}", // temp
                Hitpoints = 3,
                Damage = 3,
                DefenceRating = 15,
                WorldLocation = worldLocation,
                RegionLocation = regionLocation
            };
        }

        public override bool Adventure(Game game)
        {
            return base.Adventure(game);
        }
    }
}
