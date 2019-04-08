using Count.Models;

namespace Count.Controllers
{
    public abstract class StructureController : LocationObjectController
    {
        protected Structure _structure { get { return _object as Structure; }}

        public virtual string Name { get { return _structure.Name; } }
        public virtual string Description { get { return _structure.Description; } }

        /// <summary>
        /// Abstract upkeep method
        /// </summary>
        public abstract bool Upkeep(Game game);

        public StructureController(Location worldLocation) : base(worldLocation)
        {}
    }
}
