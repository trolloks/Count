using Count.Models;
using Newtonsoft.Json;

namespace Count.Controllers
{
    public class StructureController<T> : LocationObjectController<T> where T : Structure
    {
        public virtual string Name { get { return _object.Name; } }
        public virtual string Description { get { return _object.Description; } }

        public StructureController(Location worldLocation, Location regionLocation): base(worldLocation, regionLocation)
        {}

        public StructureController Convert()
        {
            var structureController = new StructureController(_object.WorldLocation, _object.RegionLocation);
            structureController.GenericType = this.GetType();
            structureController.RawGenericData = JsonConvert.SerializeObject(this);
            structureController.Object = _object;
            return structureController;
        }
    }
}
