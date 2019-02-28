using Count.Models;
using Count.Models.Followers;
using Count.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Count.Controllers
{
    public class CastleController : LocationObjectController
    {
        private Castle _castle;

        // Research
        public readonly Dictionary<int, ResearchItem []> ResearchOptions = new Dictionary<int, ResearchItem []>()
        {
            { 5 , new ResearchItem [] { new ResearchItem{ Name = "Graveyard", Unlocks = typeof(GraveyardController) } } }
        };

        public CastleController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
            _castle = new Castle()
            {
                Name = "Castle Varrak",
                ResearchPoints = 0
            };
        }

        #region "Actions"
        public ResearchItem[] Research(VampireLordController vampire, int soulMax)
        {
            if (ResearchOptions.ContainsKey(_castle.ResearchPoints + Math.Min(vampire.Souls, soulMax)))
            {
                _castle.ResearchPoints += Math.Min(vampire.Souls, soulMax);
                vampire.SpendSouls(Math.Min(vampire.Souls, soulMax));
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
            var follower = (VampireController)FollowerController.TryCreateFollower(typeof(Vampire), _castle.WorldLocation, _castle.RegionLocation, force);
            if (follower != null)
                _followers.Add(follower);
            return follower;
        }

        public override void Upkeep(VampireLordController vampire, List<VillageController> knownVillages, WorldController worldController)
        {
            if (knownVillages.Any())
            {
                int feeded = 0;
                foreach (var followerController in _followers)
                {
                    var vampireController = followerController as VampireController;
                    vampireController.MoveToVillage(knownVillages.OrderBy(i => Randomizer.Instance.Random.Next()).FirstOrDefault()); // Go to random village
                    var feedstatus = vampireController.Feed(worldController, vampire); // vampires feed and give you souls
                    switch (feedstatus)
                    {
                        case Enums.FeedStatus.FED:
                            feeded++;
                            break;
                    }
                }

                if (feeded > 0)
                    Console.WriteLine($"{feeded} Vampire/s Fed Successfully! Granting you {feeded} extra souls!");
            }
        }
        #endregion

        #region "Properties"
        public override string Name { get { return _castle.Name; } }
        public int ResearchPoints { get { return _castle.ResearchPoints; } }
        #endregion




    }
}
