using Count.Models;

namespace Count.Controllers
{
    public abstract class LocationObjectController
    {
        protected LocationObject _object;
        protected LocationObjectController(Location worldLocation) { }

        public Location WorldLocation { get { return _object.WorldLocation; } }
        
    }
}
