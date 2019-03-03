using Count.Controllers;
using System.Collections.Generic;

namespace Count.Models
{
    public class Game
    {
        /// <summary>
        /// You, the vampire lord
        /// </summary>
        public VampireLordController VampireLord { get; set; }
        /// <summary>
        /// Your castle
        /// </summary>
        public CastleController Castle { get; set; }
        /// <summary>
        /// The world you interact with
        /// </summary>
        public WorldController World { get; set; }

        public List<Location> KnownLocations { get; set; } = new List<Location>();
        public List<VillageController> KnownVillages { get; set; } = new List<VillageController>();
        public List<LocationObjectController> OwnedBuildings { get; set; } = new List<LocationObjectController>();
        public List<ResearchItem> KnownResearch { get; set; } = new List<ResearchItem>();

        public Location StartingWorldLocation { get; set; }
        public Location StartingRegionLocation { get; set; }

    }
}
