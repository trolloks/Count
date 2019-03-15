using Count.Models;

namespace Count.Controllers
{
    public abstract class StructureController : LocationObjectController
    {
        protected Structure _structure { get { return _object as Structure; }}

        public virtual string Name { get { return _structure.Name; } }
        public virtual string Description { get { return _structure.Description; } }

        public StructureController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {}
    }
}
