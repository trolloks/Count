using System;
using System.Collections.Generic;
using Count.Models;
using Count.Models.Followers;

namespace Count.Controllers
{
    public class UnexploredController : LocationObjectController
    {
        public UnexploredController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation){}

        public override string Description { get { return ""; } }
        public override string Name { get { return "Unexplored Area"; } }

        public override bool Upkeep(Models.Game game)
        {
            return false;
        }

        public LocationObjectController Explore(List<Location> knownLocations, WorldController worldController)
        {
            LocationObjectController locationObject = worldController.GetRegion(WorldLocation).GetLocationObjectAtLocation(RegionLocation);
            knownLocations.Add(RegionLocation);
            return locationObject;
        }
    }
}
