using System;
using Count.Models;
using Newtonsoft.Json;

namespace Count.Controllers
{
    public class StructureController : LocationObjectController<Structure>
    {
        public virtual string Name { get { return _object.Name; } }
        public virtual string Description { get { return _object.Description; } }

        public StructureController(Location worldLocation, Location regionLocation): base(worldLocation, regionLocation)
        {}
        
        public Structure GenericObject
        {
            set { _object = value; }
        }

        public Type GenericType { get; set; }
        public object Data { get; set; }

        public T Get<T, S>() where T : StructureController<S> where S : Structure
        {
            return (T)Data;
        }
    }
}
