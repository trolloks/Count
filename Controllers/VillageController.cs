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
        int villagerCounter = 1;

        public static float SUSPICION_WARNING_THRESHOLD = 0.5f;

        public VillageController(WorldController worldController)
        {
            _worldController = worldController;

            // Create village
            Village = new Village()
            {
                Id = Guid.NewGuid(),
                Name = $"Village-{_worldController.Villages.Count + 1}",
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
            Village.Villagers.Add(new Villager
            {
                Name = $"Villager-{villagerCounter}",
                Strength = (Randomizer.Instance.Roll(6, 20) / 6), // Avg
                Intelligence = (Randomizer.Instance.Roll(6, 20) / 6) // Avg
            });
            villagerCounter++;
        }

        public void KillVillager(Villager villager)
        {
            Village.Villagers.Remove(villager);
        }

        public Villager RandomVillager()
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

        public string Name
        {
            get { return Village.Name; }
        }

        public void IncreaseSuspicion()
        {
            Village.Suspicion = Math.Min(1, Village.Suspicion + (Randomizer.Instance.Roll(20, 5) / 100f)); // Can't get more suspicious than 1
        }

        public void DecreaseSuspicion()
        {
            Village.Suspicion = Math.Max(0, Village.Suspicion - (Randomizer.Instance.Roll(3, 10) / 100f)); // Can't get less suspicious than 0 
        }
    }
}
