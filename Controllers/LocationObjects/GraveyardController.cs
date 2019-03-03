using System;
using System.Linq;
using Count.Models;
using Count.Models.Followers;
using Count.Utils;

namespace Count.Controllers
{
    public class GraveyardController : LocationObjectController
    {
        private Graveyard _graveyard;
        private const int ZOMBIE_MAX = 5;

        public GraveyardController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
            _graveyard = new Graveyard()
            {
                Name = "Zombie Graveyard",
                Description = $"The graveyard is void of all life, but this doesn't mean there is none to serve you. Zombies will rise from these graves every night." +
                $"\n Can support {ZOMBIE_MAX} Zombies",
                WorldLocation = worldLocation,
                RegionLocation = regionLocation
            };
        }

        public override string Description { get { return _graveyard.Description; } }
        public override string Name { get { return _graveyard.Name; } }

        public override void Upkeep(Models.Game game)
        {
            // create new zombie!
            if (game.KnownLocations.Any(i => i.X == _graveyard.RegionLocation.X && i.Y == _graveyard.RegionLocation.Y))
            {
                // Only if you havent reached max capacity
                if (_followers.Count < ZOMBIE_MAX)
                {
                    var follower = (ZombieController)FollowerController.TryCreateFollower(typeof(Zombie), _graveyard.WorldLocation, _graveyard.RegionLocation, true);
                    if (follower != null)
                    {
                        _followers.Add(follower);
                        Console.WriteLine("A Zombie rises from the graveyard");
                    }
                }
            }

            // move zombies
            foreach (var follower in Followers)
            {
                Location location = game.KnownLocations.OrderBy(i => Randomizer.Instance.Random.Next()).FirstOrDefault();
                (follower as ZombieController).MoveToLocation(_graveyard.WorldLocation, location);
            }
        }
    }
}
