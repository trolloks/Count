using Count.Models;
using Count.Models.Followers;

namespace Count.Controllers
{
    public class ZombieController : FollowerController
    {
        public ZombieController(Location worldLocation) : base(worldLocation)
        {
            _object = new Zombie() { Name = "Zombie", WorldLocation = worldLocation, Damage = 1, Hitpoints = 1, DefenceRating = 7 };
        }
    }
}
