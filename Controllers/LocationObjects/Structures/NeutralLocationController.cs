using Count.Models;

namespace Count.Controllers
{
    public abstract class NeutralLocationController : StructureController
    {
        public NeutralLocationController(Location worldLocation) : base(worldLocation)
        { }
    }
}

