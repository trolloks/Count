using Count.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count.Controllers
{
    public class HeroController
    {
        private Hero _hero;

        public HeroController(Location worldLocation, Location regionLocation)
        {
            _hero = new Hero()
            {
                Name = $"Hero-{Guid.NewGuid().ToString()}", // temp
                Hitpoints = 3,
                WorldLocation = worldLocation,
                RegionLocation = regionLocation
            };
        }

        public Location WorldLocation { get { return _hero.WorldLocation; }  }
        public Location RegionLocation { get { return _hero.RegionLocation; } }
        public string Name { get { return _hero.Name; } }

        public void MoveToLocation(Location worldLocation, Location regionLocation)
        {
            _hero.WorldLocation = worldLocation;
            _hero.RegionLocation = regionLocation;
        }

        public bool Hero(Models.Game game)
        {
            var locationObject = game.World.GetRegion(_hero.WorldLocation).GetLocationObjectAtLocation(_hero.RegionLocation);
            var graveyards = game.OwnedBuildings.Where(i => i.GetType() == typeof(GraveyardController));
            foreach(var graveyard in graveyards)
            {
                var zombieCount = graveyard.Followers.Count;
                for (int i = 0; i < zombieCount; i++)
                {
                    var follower = graveyard.Followers[i];
                    var followerModel = follower.Follower;
                    // If this follower is at same spot as hero.
                    if (_hero.RegionLocation.X == followerModel.RegionLocation.X && _hero.RegionLocation.Y == followerModel.RegionLocation.Y
                        && _hero.WorldLocation.X == followerModel.WorldLocation.X && _hero.WorldLocation.Y == followerModel.WorldLocation.Y)
                    {
                        // hero not dead yet
                        if (_hero.Hitpoints > 0)
                        {
                            _hero.Hitpoints--;
                            graveyard.KillFollower(follower);
                            zombieCount--;
                            Console.WriteLine($"{_hero.Name} kills a Zombie, but takes 1 damage in response");
                        }
                        else
                        {
                            Console.WriteLine($"{_hero.Name} dies valiantly. You are safe against this hero.");
                            return false;
                        }
                    }
                }
            }

            // attacks you
            if (locationObject != null && locationObject.GetType() == typeof(CastleController) && _hero.Hitpoints > 0)
            {
                _hero.Hitpoints--;
                game.VampireLord.Damage(1);
                Console.WriteLine($"{_hero.Name} makes his way towards your chamber and damages you!");
            }

            if (_hero.Hitpoints > 0)
            {
                return true;
            }
            return false;
        }
    }
}
