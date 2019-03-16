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


        /// <summary>
        /// Zombies do the following:
        /// -   They shamble to known locations and settle there for the night and protect the location from any would-be heroes
        /// -   Kill villagers if they end up in village (TO BE ADDED)
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
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
                follower.MoveToLocation(_object.WorldLocation, location);

                // moved to village!
                if (game.KnownVillages.Any(i => LocationUtil.CompareLocations(i.WorldLocation, follower.WorldLocation) && LocationUtil.CompareLocations(i.RegionLocation, follower.RegionLocation)))
                {
                    var village = game.KnownVillages.FirstOrDefault(i => LocationUtil.CompareLocations(i.WorldLocation, follower.WorldLocation) && LocationUtil.CompareLocations(i.RegionLocation, follower.RegionLocation)) as VillageController; 
                    if (village.Size > 0)
                    {
                        Console.WriteLine($"A Zombie starts to attack {village.Name}");
                        while (village.Heroes.Count > 0 && follower.Hitpoints > 0)
                        {
                            Console.WriteLine($"{village.Heroes[0].Name} steps forward to defend {village.Name}");
                            CreatureController.Fight(follower.Follower, village.Heroes[0].Hero);
                            if (village.Heroes[0].Hitpoints <= 0)
                            {
                                var hero = village.Heroes[0];
                                village.KillHero(hero);
                                Console.WriteLine($"{hero.Name} dies valiantly!");
                            }

                        }

                        if (follower.Hitpoints > 0)
                        {
                            // If zombie still around kill villagers equal to a max of its damage
                            var killCount = Randomizer.Instance.Roll(1, follower.Damage);
                            Console.WriteLine($"The Zombie kills a {killCount} villagers");
                            for (int i = 0; i < killCount; i++)
                            {
                                village.TryKillVillager();
                            }
                        }
                    }
                    
                }
            }

            return somethingHappened;
        }
    }
}
