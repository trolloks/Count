using Count.Models;
using Newtonsoft.Json;

namespace Count.Controllers
{
    public abstract class StructureController<T> : LocationObjectController<T> where T : Structure
    {
        public virtual string Name { get { return _object.Name; } }
        public virtual string Description { get { return _object.Description; } }

        public StructureController(Location worldLocation, Location regionLocation): base(worldLocation, regionLocation)
        {}

        protected StructureController _structureController;

        public StructureController Convert()
        {
            _structureController = new StructureController(_object.WorldLocation, _object.RegionLocation);
            _structureController.GenericObject = _object;
            _structureController.GenericType = this.GetType();
            _structureController.RawSerializedData = JsonConvert.SerializeObject(this);
            return _structureController;
        }
    }
}
