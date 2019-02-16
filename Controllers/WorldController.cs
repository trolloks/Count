using Count.Models;
using Count.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Count.Controllers
{
    public class WorldController
    {
        private const int WORLD_SIZE = 10;

        private List<VillageController> _villages { get; }
        private World World { get; set; }

        /// <summary>
        /// Index pointing to current village
        /// </summary>
        private VillageController _currentVillage;

        public WorldController()
        {
            World = new World();
            World.Size = WORLD_SIZE; //WORLD_SIZExWORLD_SIZE
            World.Day = 1;

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

        public int Size
        {
            get { return World.Size; }
        }

        public VillageController GetCurrentVillage()
        {
            return Villages.FirstOrDefault(i => i.Equals(_currentVillage));
        }

        public void SetCurrentVillage(VillageController village)
        {
            _currentVillage = village;
        }

        public Location GenerateWorldLocation()
        {
            return new Location(Randomizer.Instance.Roll(1, Size), Randomizer.Instance.Roll(1, Size));
        }

        /// <summary>
        /// The day 
        /// </summary>
        public int Day
        {
            get { return World.Day; }
        }

        /// <summary>
        /// Move to the next day
        /// </summary>
        public void FinishDay()
        {
            World.Day++;
        }

        /// <summary>
        /// Try to find the vampire 
        /// </summary>
        /// <param name="vampireLocation">The "unknown" location of the vampired being searched</param>
        /// <returns>True, if vampire location is found</returns>
        public bool Search(Location vampireLocation)
        {
            var searchLocation = GenerateWorldLocation();
            while (World.VampireLocationsSearched.Any(i => i.X == vampireLocation.X && i.Y == vampireLocation.Y))
                searchLocation = GenerateWorldLocation();

            if (Game.IS_DEV)
            {
                Console.WriteLine($"(DEV) HIDING AT: {vampireLocation.X}:{vampireLocation.Y}");
                if (IsVampireLocationFound)
                    Console.WriteLine($"(DEV) WORLD FOUND:  {searchLocation.X}:{searchLocation.Y}");
                else
                {
                    Console.WriteLine($"(DEV) WORLD PREVIOUSLY SEARCHED:  {LocationUtil.ToStringFromLocations(World.VampireLocationsSearched)}");
                    Console.WriteLine($"(DEV) WORLD SEARCHED:  {searchLocation.X}:{searchLocation.Y}");
                }
            }

            if ((vampireLocation.X == searchLocation.X && vampireLocation.Y == searchLocation.Y) || IsVampireLocationFound)
            {
                World.IsVampireLocationFound = true;
                return true;
            }

            // Search better each round
            World.VampireLocationsSearched.Add(searchLocation);
            return false;
        }

        public bool IsVampireLocationFound
        {
            get { return World.IsVampireLocationFound; }
        }

        public void InvalidateSearch()
        {
            World.IsVampireLocationFound = false;
            // Instead of clearing.. remove only x amount of searched locations
            World.VampireLocationsSearched.Clear();
        }
    }
}
