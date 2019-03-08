using System;
using Count.Models;

namespace Count.Controllers
{
    public abstract class HeroController : CreatureController<Hero>
    {
        public HeroController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
            _object = new Hero()
            {
                Name = $"Hero-{Guid.NewGuid().ToString()}", // temp
                Hitpoints = 3,
                WorldLocation = worldLocation,
                RegionLocation = regionLocation
            };
        }

        public string Name { get { return _object.Name; } }

        public abstract bool Hero(Models.Game game);
    }
}
