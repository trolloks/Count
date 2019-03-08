using System;
using System.Linq;
using Count.Models;

namespace Count.Controllers
{
    public abstract class HeroController
    {
        protected Hero _hero;

        public HeroController(Location worldLocation, Location regionLocation)
        {
            _hero = new Hero()
            {
                Name = $"Hero-{Guid.NewGuid().ToString()}", // temp
                Hitpoints = 3,
                WorldLocation = worldLocation,
                RegionLocation = regionLocation
            };
        }

        public Location WorldLocation { get { return _hero.WorldLocation; } }
        public Location RegionLocation { get { return _hero.RegionLocation; } }
        public string Name { get { return _hero.Name; } }

        public void MoveToLocation(Location worldLocation, Location regionLocation)
        {
            _hero.WorldLocation = worldLocation;
            _hero.RegionLocation = regionLocation;
        }

        public abstract bool Hero(Models.Game game);
    }
}
