using Count.Models;
using System;
using System.Collections.Generic;

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
        #endregion

        #region "Properties"
        public override string Name { get { return _castle.Name; } }
        public int ResearchPoints { get { return _castle.ResearchPoints; } }
        #endregion




    }
}
