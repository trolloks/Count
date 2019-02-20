using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Count.Models;

namespace Count.Controllers
{
    public class RegionController
    {
        private const int REGION_SIZE = 10; // Not used yet

        private List<VillageController> _villages { get; }
        private WorldController _worldController;
        private Region Region { get; set; }

        /// <summary>
        /// Index pointing to current village
        /// </summary>
        private VillageController _currentVillage;

        public RegionController(WorldController worldController)
        {
            _worldController = worldController;

            Region = new Region();

            _villages = new List<VillageController>();

            // Create first village
            _villages.Add(new VillageController(this));
            _currentVillage = _villages[0];
        }

        public ReadOnlyCollection<VillageController> Villages
        {
            get { return _villages.AsReadOnly(); }
        }

        public VillageController AddVillage()
        {
            var village = new VillageController(this);
            _villages.Add(village);
            return village;
        }

        public VillageController GetCurrentVillage()
        {
            return Villages.FirstOrDefault(i => i.Equals(_currentVillage));
        }

        public void SetCurrentVillage(VillageController village)
        {
            _currentVillage = village;
        }
    }
}
