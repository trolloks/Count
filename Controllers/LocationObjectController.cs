using Count.Models;

namespace Count.Controllers
{
    public abstract class LocationObjectController
    {
        protected LocationObject _object;
        protected LocationObjectController(Location worldLocation, Location regionLocation) { }

        public Location WorldLocation { get { return _object.WorldLocation; } }
        public Location RegionLocation { get { return _object.RegionLocation; } }
        
    }
}
