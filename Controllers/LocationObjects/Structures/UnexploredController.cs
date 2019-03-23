using Count.Models;

namespace Count.Controllers
{
    public class UnexploredController : StructureController
    {
        public UnexploredController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation) {
            _object = new Structure
            {
                WorldLocation = worldLocation,
                RegionLocation = regionLocation,
                Description = "",
                Name = "Unexplored Area"
            };
        }

        public StructureController Explore(Game game)
        {
            StructureController locationObject = game.World.GetRegion(WorldLocation).GetLocationObjectAtLocation(RegionLocation);
            game.KnownLocations.Add(RegionLocation);
            if (locationObject != null && locationObject.GetType().IsSubclassOf(typeof(FriendlyLocationController)))
            {
                game.OwnedBuildings.Add(locationObject);
            }
            return locationObject;
        }

        public override bool Upkeep(Game game)
        {
            // nothing interesting
            return false;
        }
    }
}
