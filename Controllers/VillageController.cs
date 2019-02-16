using Count.Models;
using Count.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Count.Controllers
{
    public class VillageController
    {
        private WorldController _worldController;

        public Village Village { get; set; }
        

        // temp
        int villagerCounter = 0;

        public VillageController(WorldController worldController)
        {
            _worldController = worldController;

            // Create village
            Village = new Village()
            {
                Villagers = new List<Villager>(),
                Suspicion = 0
            };

            var numVillagers = Randomizer.Instance.Roll(40, 2);
            for (int i = 0; i < numVillagers; i++)
            {
                SpawnVillager();
            }
        }

        public void SpawnVillager()
        {
            Village.Villagers.Add(new Villager { Name = $"Villager-{villagerCounter}" });
            villagerCounter++;
        }

        public void KillVillager()
        {
            // Random villager
            var unluckyPerson = Randomizer.Instance.Random.Next(Size);
            Village.Villagers.RemoveAt(unluckyPerson);
        }

        public int Size
        {
            get { return Village.Villagers.Count; }
        }

        public float Suspicion
        {
            get { return Village.Suspicion; }
        }

        public bool IsLocationFound
        {
            get { return Village.IsLocationFound; }
        }

        /// <summary>
        /// Try to find the vampire 
        /// </summary>
        /// <param name="vampireLocation">The "unknown" location of the vampired being searched</param>
        /// <returns>True, if vampire location is found</returns>
        public bool Search(Location vampireLocation) 
        {
            var searchLocation = _worldController.GenerateWorldLocation();
            while (Village.LocationsSearched.Any(i => i.X == vampireLocation.X && i.Y == vampireLocation.Y))
                searchLocation = _worldController.GenerateWorldLocation();

            if (Game.IS_DEV)
            {
                Console.WriteLine($"(DEV) HIDING AT: {vampireLocation.X}:{vampireLocation.Y}");
                if (Village.IsLocationFound)
                    Console.WriteLine($"(DEV) VILLAGERS FOUND:  {searchLocation.X}:{searchLocation.Y}");
                else
                {
                    Console.WriteLine($"(DEV) VILLAGERS PREVIOUSLY SEARCHED:  {LocationUtil.ToStringFromLocations(Village.LocationsSearched)}");
                    Console.WriteLine($"(DEV) VILLAGERS SEARCHED:  {searchLocation.X}:{searchLocation.Y}");
                }
            }

            if ((vampireLocation.X == searchLocation.X && vampireLocation.Y == searchLocation.Y) || Village.IsLocationFound)
            {
                Village.IsLocationFound = true;
                return true;
            }

            // Search better each round
            Village.LocationsSearched.Add(searchLocation);
            return false;
        }

        public void InvalidateSearch()
        {
            Village.IsLocationFound = false;
            Village.LocationsSearched.Clear();
        }

        public void IncreaseSuspicion()
        {
            Village.Suspicion = Math.Min(1, Village.Suspicion + (Randomizer.Instance.Roll(3, 10) / 100f)); // Can't get more suspicious than 1
        }

        public void DecreaseSuspicion()
        {
            Village.Suspicion = Math.Max(0, Village.Suspicion - (Randomizer.Instance.Roll(20, 5) / 100f)); // Can't get less suspicious than 0 
        }
    }
}
