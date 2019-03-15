using Count.Models;

namespace Count.Controllers
{
    public abstract class EnemyLocationController : StructureController
    {
        public EnemyLocationController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        { }
    }
}

