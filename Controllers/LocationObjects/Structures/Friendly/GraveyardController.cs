using System;
using System.Linq;
using Count.Models;
using Count.Models.Followers;
using Count.Utils;

namespace Count.Controllers
{
    public class GraveyardController : FriendlyLocationController
    {

        public GraveyardController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
            _object = new Graveyard()
            {
                Name = "Graveyard",
                Description = $"Filled with numerous graves of villagers. Somehow this could be used to your advantage",
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
            // only if location is known
            if (game.KnownLocations.Any(i => i.X == _object.RegionLocation.X && i.Y == _object.RegionLocation.Y))
            {
                // only if "raise zombie" has been researched
                if (game.KnownResearch.Any(i => i.Unlocks == GetType()))
                {
                    if (game.VampireLord.Corpses > 0)
                    {
                        int corpsesToUse = Math.Min(10, game.VampireLord.Corpses); // 10 is max zombies that can be risen at one time
                        for (int i = 0; i < corpsesToUse; i++)
                        {
                            var follower = FollowerController.TryCreateFollower(typeof(Zombie), _object.WorldLocation, _object.RegionLocation, true);
                            if (follower != null)
                            {
                                _followers.Add(follower);
                                
                                // use corpse to create zombie
                                game.VampireLord.SpendCorpses(1);
                            }
                        }
                        Console.WriteLine($"You raise {corpsesToUse} new zombie/s from a corpse/s in the graveyard");
                    }
                    else
                    {
                        Console.WriteLine($"You did not have enough corpses to create another.");
                    }
                    somethingHappened = true;
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
                            Console.WriteLine($"The Zombie kills {killCount} villager/s");
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
