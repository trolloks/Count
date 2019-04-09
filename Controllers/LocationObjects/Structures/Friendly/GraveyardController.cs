using System;
using System.Linq;
using Count.Models;
using Count.Models.Followers;
using Count.Utils;

namespace Count.Controllers
{
    public class GraveyardController : FriendlyLocationController
    {

        public GraveyardController(Location worldLocation) : base(worldLocation)
        {
            _object = new Graveyard()
            {
                Name = "Graveyard",
                Description = $"Filled with numerous graves of villagers. Somehow this could be used to your advantage",
                WorldLocation = worldLocation
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
            if (game.World.Locations.Any(i => i.X == _object.WorldLocation.X && i.Y == _object.WorldLocation.Y))
            {
                // only if "raise zombie" has been researched
                if (game.KnownResearch.Any(i => i.Unlocks == GetType()))
                {
                    if (game.VampireLord.Corpses > 0)
                    {
                        int corpsesToUse = Math.Min(10, game.VampireLord.Corpses); // 10 is max zombies that can be risen at one time
                        for (int i = 0; i < corpsesToUse; i++)
                        {
                            var follower = FollowerController.TryCreateFollower(typeof(Zombie), _object.WorldLocation, true);
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
                Location location = game.World.Locations.OrderBy(i => Randomizer.Instance.Random.Next()).FirstOrDefault();
                follower.MoveToLocation(location);
            }

            return somethingHappened;
        }
    }
}
