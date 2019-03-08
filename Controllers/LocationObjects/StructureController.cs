using Count.Models;

namespace Count.Controllers
{
    public abstract class StructureController<T> : LocationObjectController<T> where T : Structure
    {
        public virtual string Name { get { return _object.Name; } }
        public virtual string Description { get { return _object.Description; } }

        public StructureController(Location worldLocation, Location regionLocation): base(worldLocation, regionLocation)
        {}
    }
}
