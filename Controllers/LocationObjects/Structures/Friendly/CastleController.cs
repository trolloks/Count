using Count.Models;
using Count.Models.Followers;
using Count.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Count.Controllers
{
    public class CastleController : FriendlyLocationController
    {
        protected Castle _castle { get { return _object as Castle; } }

        // Research
        public readonly Dictionary<int, ResearchItem []> ResearchOptions = new Dictionary<int, ResearchItem []>()
        {
            { 5 , new ResearchItem [] { new ResearchItem{ Name = "Graveyard", Unlocks = typeof(GraveyardController), Souls = 5 } } }
        };

        public CastleController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
            _object = new Castle()
            {
                Name = "Castle Varrak",
                ResearchPoints = 0,
                WorldLocation = worldLocation,
                RegionLocation = regionLocation
            };
        }

        #region "Actions"
        public ResearchItem[] Research(int soulsCurrent, int soulMax)
        {
            if (ResearchOptions.ContainsKey(_castle.ResearchPoints + Math.Min(soulsCurrent, soulMax)))
            {
                _castle.ResearchPoints += Math.Min(soulsCurrent, soulMax);
                var researchItems = ResearchOptions[_castle.ResearchPoints];
                foreach (var researchItem in researchItems)
                    _castle.UnlockedResearch.Add(researchItem);
                return researchItems;
            }
            else
                return null;
        }

        public VampireController CreateVampire()
        {
            return CreateVampire(false);
        }

        public VampireController CreateVampire(bool force)
        {
            var follower = (VampireController)FollowerController.TryCreateFollower(typeof(Vampire), _object.WorldLocation, _object.RegionLocation, force);
            if (follower != null)
                _followers.Add(follower);
            return follower;
        }

        /// <summary>
        /// Vampires do the following:
        /// -   They go to a random known village and try to feed. If they succeed you gain some of the souls
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public override bool Upkeep(Models.Game game)
        {
            bool somethingHappened = false;
            if (game.KnownVillages.Any())
            {
                int fed = 0;
                foreach (var followerController in _followers)
                {
                    var vampireController = followerController as VampireController;
                    var village = game.KnownVillages.OrderBy(i => Randomizer.Instance.Random.Next()).FirstOrDefault();
                    vampireController.MoveToLocation(village.WorldLocation, village.RegionLocation); // Go to random village
                    var feedstatus = vampireController.Feed(game.World, game.VampireLord); // vampires feed and give you souls
                    switch (feedstatus)
                    {
                        case Enums.FeedStatus.FED:
                            fed++;
                            break;
                    }
                    // Move vampire back to castle
                    vampireController.MoveToLocation(_object.WorldLocation, _object.RegionLocation); 
                }

                if (fed > 0)
                {
                    Console.WriteLine($"{fed} Vampire/s Fed Successfully! Granting you {fed} extra souls!");
                    somethingHappened = true;
                } 
            }

            return somethingHappened;
        }
        #endregion

        #region "Properties"
        public int ResearchPoints { get { return _castle.ResearchPoints; } }
        #endregion
    }
}
