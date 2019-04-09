using System;
using System.Collections.Generic;
using Count.Models;

namespace Count.Controllers
{
    public class CastleController : FriendlyLocationController
    {
        protected Castle _castle { get { return _object as Castle; } }

        // Research
        public readonly Dictionary<int, ResearchItem[]> ResearchOptions = new Dictionary<int, ResearchItem[]>()
        {
            { 10 , new ResearchItem [] { new ResearchItem{ Name = "Raise Dead", Description = "Corpses from dead humans will rise as zombies at discovered graveyards.", Unlocks = typeof(GraveyardController) } } }
        };

        public CastleController(Location worldLocation) : base(worldLocation)
        {
            _object = new Castle()
            {
                Name = "Castle Varrak",
                ResearchPoints = 0,
                WorldLocation = worldLocation
            };
        }

        #region "Actions"
        public ResearchItem[] Research(int bloodCurrent, int bloodMax)
        {
            if (ResearchOptions.ContainsKey(_castle.ResearchPoints + Math.Min(bloodCurrent, bloodMax)))
            {
                _castle.ResearchPoints += Math.Min(bloodCurrent, bloodMax);
                var researchItems = ResearchOptions[_castle.ResearchPoints];
                foreach (var researchItem in researchItems)
                    _castle.UnlockedResearch.Add(researchItem);
                return researchItems;
            }
            else
                return null;
        }

        public override bool Upkeep(Models.Game game)
        {
            // Do stuff during the night.
            return false;
        }
        #endregion

        #region "Properties"
        public int ResearchPoints { get { return _castle.ResearchPoints; } }
        #endregion
    }
}
