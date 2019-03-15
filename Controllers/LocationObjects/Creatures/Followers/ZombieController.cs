using Count.Models;
using Count.Models.Followers;

namespace Count.Controllers
{
    public class ZombieController : FollowerController
    {
        public ZombieController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
            _object = new Zombie() { WorldLocation = worldLocation, RegionLocation = regionLocation, Attack = 1, Hitpoints = 1 };
        }
    }
}
