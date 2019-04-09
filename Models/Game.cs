using Count.Controllers;
using System.Collections.Generic;

namespace Count.Models
{
    public class Game
    {
        public static int HERO_MAX = 1;


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
        public List<StructureController> OwnedBuildings { get; set; } = new List<StructureController>();
        public List<ResearchItem> KnownResearch { get; set; } = new List<ResearchItem>();
        public List<HeroController> Heroes { get; set; } = new List<HeroController>();

        public Location StartingWorldLocation { get; set; }
    }
}
