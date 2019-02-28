using System.Collections.Generic;
using Count.Models;
using Count.Models.Followers;

namespace Count.Controllers
{
    public class GraveyardController : LocationObjectController
    {
        private Graveyard _graveyard;

        public GraveyardController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
            _graveyard = new Graveyard()
            {
                Name = "Zombie Graveyard",
                Description = "The graveyard is void of all life, but this doesn't mean there is none to serve you. Zombies will rise from these graves every night."
            };
        }

        public override string Description { get { return _graveyard.Description; } }
        public override string Name { get { return _graveyard.Name; } }

        public override void Upkeep(VampireLordController vampire, List<VillageController> knownVillages, WorldController worldController)
        {
            var follower = (ZombieController)FollowerController.TryCreateFollower(typeof(Zombie), _graveyard.WorldLocation, _graveyard.RegionLocation);
            if (follower != null)
            {
                _followers.Add(follower);
            }
        }
    }
}
