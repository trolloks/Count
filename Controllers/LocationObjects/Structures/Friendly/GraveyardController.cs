using System;
using System.Linq;
using Count.Models;
using Count.Models.Followers;
using Count.Utils;

namespace Count.Controllers
{
    public class GraveyardController : FriendlyLocationController
    {
        private const int ZOMBIE_MAX = 5;

        public GraveyardController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
            _object = new Graveyard()
            {
                Name = "Zombie Graveyard",
                Description = $"The graveyard is void of all life, but this doesn't mean there is none to serve you. Zombies will rise from these graves every night." +
                $"\n Can support {ZOMBIE_MAX} Zombies",
                WorldLocation = worldLocation,
                RegionLocation = regionLocation
            };
        }

        public override bool Upkeep(Models.Game game)
        {
            bool somethingHappened = false;
            // create new zombie!
            if (game.KnownLocations.Any(i => i.X == _object.RegionLocation.X && i.Y == _object.RegionLocation.Y))
            {
                // Only if you havent reached max capacity
                if (_followers.Count < ZOMBIE_MAX)
                {
                    var follower = FollowerController.TryCreateFollower(typeof(Zombie), _object.WorldLocation, _object.RegionLocation, true);
                    if (follower != null)
                    {
                        _followers.Add(follower);
                        Console.WriteLine("A Zombie rises from the graveyard");
                        somethingHappened = true;
                    }
                }
            }

            // move zombies
            foreach (var follower in Followers)
            {
                Location location = game.KnownLocations.OrderBy(i => Randomizer.Instance.Random.Next()).FirstOrDefault();
                (follower as ZombieController).MoveToLocation(_object.WorldLocation, location);
            }

            return somethingHappened;
        }
    }
}
