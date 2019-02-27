using Count.Models;

namespace Count.Controllers
{
    public class GraveyardController : LocationObjectController
    {
        public GraveyardController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        { }
    }
}
