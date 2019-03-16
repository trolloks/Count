using System;
using System.Linq;
using Count.Models;

namespace Count.Controllers
{
    public class FighterController : HeroController
    { 

        public FighterController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
            _object = new Fighter()
            {
                Name = $"Hero-{Guid.NewGuid().ToString()}", // temp
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
