using Count.Models;

namespace Count.Controllers
{
    public abstract class LocationObjectController
    {
        public Location WorldLocation { get; set; }
        public Location RegionLocation { get; set; }
        public virtual string Name { get; set; }

        public LocationObjectController(Location worldLocation, Location regionLocation) {
            WorldLocation = worldLocation;
            RegionLocation = regionLocation;
        }
    }
}
