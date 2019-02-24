using Count.Models;

namespace Count.Controllers
{
    public class CastleController : LocationObjectController
    {
        public override string Name { get { return "Castle Varrak"; } }

        public CastleController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        { }
    }
}
