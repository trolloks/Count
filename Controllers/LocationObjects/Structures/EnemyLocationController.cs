using Count.Models;

namespace Count.Controllers
{
    public abstract class EnemyLocationController<T> : StructureController<T> where T : Structure
    {
        public EnemyLocationController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        { }
    }
}

