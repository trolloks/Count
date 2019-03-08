using System;
using System.Linq;
using Count.Models;

namespace Count.Controllers
{
    public class FighterController : HeroController<Hero>
    {
        public FighterController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
            _object = new Fighter()
            {
                Name = $"Hero-{Guid.NewGuid().ToString()}", // temp
                Hitpoints = 3,
                WorldLocation = worldLocation,
                RegionLocation = regionLocation
            };
        }

        public override bool Hero(Game game)
        {
            var locationObject = game.World.GetRegion(_object.WorldLocation).GetLocationObjectAtLocation(_object.RegionLocation);
            var graveyards = game.OwnedBuildings.Where(i => i.GenericType == typeof(GraveyardController));
            foreach (var graveyardRaw in graveyards)
            {
                var graveyard = graveyardRaw.Convert<GraveyardController, Graveyard>();
                var zombieCount = graveyard.Followers.Count;
                for (int i = 0; i < zombieCount; i++)
                {
                    var follower = graveyard.Followers[i];
                    var followerModel = follower.Follower;
                    // If this follower is at same spot as hero.
                    if (_object.RegionLocation.X == followerModel.RegionLocation.X && _object.RegionLocation.Y == followerModel.RegionLocation.Y
                        && _object.WorldLocation.X == followerModel.WorldLocation.X && _object.WorldLocation.Y == followerModel.WorldLocation.Y)
                    {
                        // hero not dead yet
                        if (_object.Hitpoints > 0)
                        {
                            _object.Hitpoints -= followerModel.Damage;
                            graveyard.KillFollower(follower);
                            zombieCount--;
                            i--;
                            Console.WriteLine($"{_object.Name} kills a Zombie, but takes {followerModel.Damage} damage in response");
                        }
                        else
                        {
                            Console.WriteLine($"{_object.Name} dies valiantly. You are safe against this hero.");
                            return false;
                        }
                    }
                }
            }

            // attacks you
            if (locationObject != null && locationObject.GenericType == typeof(CastleController) && _object.Hitpoints > 0)
            {
                var castle = locationObject.Convert<CastleController, Castle>();
                var vampireCount = castle.Followers.Count;
                for (int i = 0; i < vampireCount; i++)
                {
                    var follower = castle.Followers[i];
                    var followerModel = follower.Follower;
                    // If this follower is at same spot as hero.
                    if (_object.RegionLocation.X == followerModel.RegionLocation.X && _object.RegionLocation.Y == followerModel.RegionLocation.Y
                        && _object.WorldLocation.X == followerModel.WorldLocation.X && _object.WorldLocation.Y == followerModel.WorldLocation.Y)
                    {
                        // hero not dead yet
                        if (_object.Hitpoints > 0)
                        {
                            _object.Hitpoints -= followerModel.Damage;
                            castle.KillFollower(follower);
                            vampireCount--;
                            i--;
                            Console.WriteLine($"{_object.Name} kills a Vampire, but takes {followerModel.Damage} damage in response");
                        }
                        else
                        {
                            Console.WriteLine($"{_object.Name} dies valiantly. You are safe against this hero.");
                            return false;
                        }
                    }
                }

                if (_object.Hitpoints > 0)
                {
                    _object.Hitpoints--;
                    game.VampireLord.ForceKill();
                    Console.WriteLine($"{_object.Name} makes his way towards your chamber and kills you!");
                }
                else
                {
                    Console.WriteLine($"{_object.Name} dies valiantly. You are safe against this hero.");
                }
            }

            if (_object.Hitpoints > 0)
            {
                return true;
            }

            return false;
        }
    }
}
