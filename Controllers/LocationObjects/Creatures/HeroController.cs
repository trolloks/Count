using System;
using System.Linq;
using Count.Models;
using Count.Utils;

namespace Count.Controllers
{
    public abstract class HeroController : CreatureController
    {
        protected Hero _hero { get { return _object as Hero; } }
        public Hero Hero { get { return _hero; } }
        
        private static int BASE_SPAWN_MIN_DAY = 10;
        private static int BASE_SPAWN_DC = 10;
        private static int BASE_CHECK_ROLL = 20;
        private static int BASE_SPAWN_PITY = 5;

        protected HeroController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation) { }


        /// <summary>
        ///  Checks if you succeed on creating a follower
        /// </summary>
        public static HeroController TryCreateHero(Type heroType, Game game, Location worldLocation, Location regionLocation, bool force)
        {
            // Roll for adventurer spawn
            // - MAX 5
            // - MUST ROLL MORE THAN 16
            // - IF SUCCEEDS CAN ROLL AGAIN
            // - only able to spawn after day 5
            // - have 'pity' counter
            var roll = Randomizer.Instance.Roll(1, BASE_CHECK_ROLL);
            var convertCheck = (roll >= BASE_SPAWN_DC && game.World.Day >= BASE_SPAWN_MIN_DAY) || force;
            if (convertCheck)
            {
                
                if (heroType == typeof(Fighter))
                    return new FighterController(worldLocation, regionLocation) as HeroController;
            }

            return null;
        }

        public virtual bool Adventure(Models.Game game)
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
                            CreatureController.Fight(_hero, followerModel);
                            if (followerModel.Hitpoints <= 0)
                            {
                                graveyard.KillFollower(follower);
                                zombieCount--;
                                i--;
                                Console.Write($"{_hero.Name} kills a Zombie");
                                if (_hero.Hitpoints > 0)
                                {
                                    Console.WriteLine($", and survives with {_hero.Hitpoints} hit points");
                                    continue;
                                }
                                else
                                    Console.WriteLine(".");
                            }
                        }

                        Console.WriteLine($"{_hero.Name} dies valiantly. You are safe against this hero.");
                        return false;
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
                            CreatureController.Fight(_hero, followerModel);
                            if (followerModel.Hitpoints <= 0)
                            {
                                castle.KillFollower(follower);
                                vampireCount--;
                                i--;
                                Console.Write($"{_hero.Name} kills a Vampire");
                                if (_hero.Hitpoints > 0)
                                {
                                    Console.WriteLine($", and survives with {_hero.Hitpoints} hit points");
                                    continue;
                                }
                                else
                                    Console.WriteLine(".");
                            }
                           
                        } 

                        Console.WriteLine($"{_hero.Name} dies valiantly. You are safe against this hero.");
                        return false;
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
