using Count.Models;

namespace Count.Controllers
{
    public class UnexploredController : StructureController<Structure>
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

        public StructureController<Structure> Explore(Game game)
        {
            StructureController<Structure> locationObject = game.World.GetRegion(WorldLocation).GetLocationObjectAtLocation(RegionLocation);
            game.KnownLocations.Add(RegionLocation);
            return locationObject;
        }
    }
}
