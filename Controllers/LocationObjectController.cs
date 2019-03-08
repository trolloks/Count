using Count.Models;

namespace Count.Controllers
{
    public abstract class LocationObjectController<T> where T : LocationObject
    {
        protected T _object;
        protected LocationObjectController(Location worldLocation, Location regionLocation) { }

        public Location WorldLocation { get { return _object.WorldLocation; } }
        public Location RegionLocation { get { return _object.RegionLocation; } }
        public T Object { get { return _object; } }
    }
}
