using Count.Models;
using Count.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count.Controllers
{
    public class WorldController
    {
        private const int WORLD_SIZE = 3;

        private List<VillageController> Villages { get; set; }
        private World World { get; set; }

        /// <summary>
        /// Index pointing to current village
        /// </summary>
        private int _currentVillageIndex;

        public WorldController()
        {
            World = new World();
            World.Size = WORLD_SIZE; //WORLD_SIZExWORLD_SIZE

            Villages = new List<VillageController>();

            // Create first village
            Villages.Add(new VillageController(this));

            _currentVillageIndex = 0;
        }

        public int Size
        {
            get { return World.Size; }
        }

        public VillageController GetCurrentVillage()
        {
            return Villages[_currentVillageIndex];
        }

        public Location GenerateWorldLocation()
        {
            return new Location(Randomizer.Instance.Roll(1, Size), Randomizer.Instance.Roll(1, Size));
        }

    }
}
