using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Count.Models;
using Count.Utils;

namespace Count.Controllers
{
    public class WorldController
    {
        private const int WORLD_SIZE = 10;

        private List<RegionController> _regions { get; }
        private World World { get; set; }

        /// <summary>
        /// Index pointing to current village
        /// </summary>
        private RegionController _currentRegion;

        public WorldController()
        {
            World = new World();
            World.Size = WORLD_SIZE; //WORLD_SIZExWORLD_SIZE
            World.Day = 1;

            _regions = new List<RegionController>();

            // Create first region
            _regions.Add(new RegionController(this));
            _currentRegion = _regions[0];
        }

        public ReadOnlyCollection<RegionController> Regions
        {
            get { return _regions.AsReadOnly(); }
        }

        public RegionController AddRegion()
        {
            var region = new RegionController(this);
            _regions.Add(region);
            return region;
        }

        public int Size
        {
            get { return World.Size; }
        }

        public RegionController GetCurrentRegion()
        {
            return Regions.FirstOrDefault(i => i.Equals(_currentRegion));
        }

        public void SetCurrentRegion(RegionController region)
        {
            _currentRegion = region;
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
