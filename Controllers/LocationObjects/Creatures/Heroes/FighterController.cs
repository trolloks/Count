using System;
using System.Linq;
using Count.Models;

namespace Count.Controllers
{
    public class FighterController : HeroController
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
            var ownedGraveyardStructures = game.OwnedBuildings.Where(i => i.GetType() == typeof(GraveyardController));
            foreach (var structure in ownedGraveyardStructures)
            {
                var graveyard = structure as GraveyardController;
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
                        if (_hero.Hitpoints > 0)
                        {
                            _hero.Hitpoints -= followerModel.Damage;
                            graveyard.KillFollower(follower);
                            zombieCount--;
                            i--;
                            Console.WriteLine($"{_hero.Name} kills a Zombie, but takes {followerModel.Damage} damage in response");
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
                var castle = locationObject as CastleController;
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
                        if (_hero.Hitpoints > 0)
                        {
                            _hero.Hitpoints -= followerModel.Damage;
                            castle.KillFollower(follower);
                            vampireCount--;
                            i--;
                            Console.WriteLine($"{_hero.Name} kills a Vampire, but takes {followerModel.Damage} damage in response");
                        }
                        else
                        {
                            Console.WriteLine($"{_hero.Name} dies valiantly. You are safe against this hero.");
                            return false;
                        }
                    }
                }

                if (_hero.Hitpoints > 0)
                {
                    _hero.Hitpoints--;
                    game.VampireLord.ForceKill();
                    Console.WriteLine($"{_hero.Name} makes his way towards your chamber and kills you!");
                }
                else
                {
                    Console.WriteLine($"{_hero.Name} dies valiantly. You are safe against this hero.");
                }
            }

            if (_hero.Hitpoints > 0)
            {
                return true;
            }

            return false;
        }
    }
}
