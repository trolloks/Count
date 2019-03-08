using Count.Models;

namespace Count.Controllers
{
    public abstract class EnemyLocationController : LocationObjectController
    {
        public EnemyLocationController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        { }
    }
}

