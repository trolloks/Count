using System;
using System.Collections.Generic;
using Count.Models;
using Count.Utils;

namespace Count.Controllers
{
    public class VillageController : LocationObjectController
    {
        private Village Village { get; set; }

        // temp (For names)
        int villagerCounter = 1;

        public static float SUSPICION_WARNING_THRESHOLD = 1f;

        public VillageController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
            // Create village
            Village = new Village()
            {
                Id = Guid.NewGuid(),
                Name = $"Village-{Guid.NewGuid().ToString()}", // temp
                Villagers = new List<Villager>(),
                WorldLocation = worldLocation,
                RegionLocation = regionLocation,
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
            Village.Villagers.Add(new Villager
            {
                Name = $"Villager-{villagerCounter}",
                Strength = (Randomizer.Instance.Roll(6, 20) / 6), // Avg
                Intelligence = (Randomizer.Instance.Roll(6, 20) / 6), // Avg
                Charisma = (Randomizer.Instance.Roll(6, 20) / 6) // Avg
            });
            villagerCounter++;
        }

        public void KillVillager()
        {
            KillVillager(RandomVillager());
        }

        private void KillVillager(Villager villager)
        {
            Village.Villagers.Remove(villager);
        }

        private Villager RandomVillager()
        {
            var unluckyPerson = Randomizer.Instance.Random.Next(Size);
            return Village.Villagers[unluckyPerson];
        }

        public int Size
        {
            get { return Village.Villagers.Count; }
        }

        public float Suspicion
        {
            get { return Village.Suspicion; }
        }

        public override string Name
        {
            get { return Village.Name; }
        }

        public void IncreaseSuspicion()
        {
            Village.Suspicion = Math.Min(1, Village.Suspicion + (Randomizer.Instance.Roll(15, 5) / 100f)); // Can't get more suspicious than 1
        }

        public void DecreaseSuspicion()
        {
            Village.Suspicion = Math.Max(0, Village.Suspicion - (Randomizer.Instance.Roll(5, 5) / 100f)); // Can't get less suspicious than 0 
        }

        public override void Upkeep(Models.Game game)
        {
           
        }
    }
}
