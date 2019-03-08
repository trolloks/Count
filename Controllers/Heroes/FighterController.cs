using System;
using System.Linq;
using Count.Models;

namespace Count.Controllers
{
    public class FighterController : HeroController
    {
        public FighterController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
        }

        public override bool Hero(Game game)
        {
            var locationObject = game.World.GetRegion(_hero.WorldLocation).GetLocationObjectAtLocation(_hero.RegionLocation);
            var graveyards = game.OwnedBuildings.Where(i => i.GetType() == typeof(GraveyardController));
            foreach (var graveyard in graveyards)
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
                    if (_hero.RegionLocation.X == followerModel.RegionLocation.X && _hero.RegionLocation.Y == followerModel.RegionLocation.Y
                        && _hero.WorldLocation.X == followerModel.WorldLocation.X && _hero.WorldLocation.Y == followerModel.WorldLocation.Y)
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
