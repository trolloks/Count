using System;
using Count.Models;
using Count.Models.Followers;

namespace Count.Controllers
{
    public class ZombieController : FollowerController
    {
        public ZombieController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
            _follower = new Zombie() { Available = true, WorldLocation = worldLocation, RegionLocation = regionLocation };
        }
    }
}
