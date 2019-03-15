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
                Attack = 3,
                WorldLocation = worldLocation,
                RegionLocation = regionLocation
            };
        }

        public override bool Hero(Game game)
        {
            return base.Hero(game);
        }
    }
}
