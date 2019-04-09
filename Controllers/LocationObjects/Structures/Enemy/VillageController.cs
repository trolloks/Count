using System;
using Count.Models;

namespace Count.Controllers
{
    public class VillageController : EnemyLocationController
    {
        private Village _village { get { return _object as Village; } }

        protected static int _globalVillageCount;
        private const int HERO_MAX = 1;
        private int _pendingCorpses;

        public VillageController(Location worldLocation) : base(worldLocation)
        {
            // Create village
            _object = new Village()
            {
                Id = Guid.NewGuid(),
                Name = $"Village",
                WorldLocation = worldLocation
            };
        }

        public override string Name
        {
            get { return _village.Name; }
        }

        public override bool Upkeep(Game game)
        {
            var somethingHappened = false;
            return somethingHappened;
        }
    }
}
