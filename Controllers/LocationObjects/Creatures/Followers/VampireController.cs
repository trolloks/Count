using Count.Models;
using Count.Models.Followers;

namespace Count.Controllers
{
    public class VampireController : FollowerController
    {
        public VampireController(Location worldLocation) : base(worldLocation)
        {
            _object = new Vampire() { Name = "Vampire", WorldLocation = worldLocation, Damage = 3, Hitpoints = 3, DefenceRating = 12 };
        }
    }
}
