using System;
using System.Collections.Generic;
using Count.Models;
using Count.Utils;

namespace Count.Controllers
{
    public class VillageController : EnemyLocationController
    {
        private Village _village { get { return _object as Village; } }

        private const int FIGHTER_MAX = 3;

        // temp (For names)
        int villagerCounter = 1;

        public static float SUSPICION_WARNING_THRESHOLD = 1f;

        public VillageController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
            // Create village
            _object = new Village()
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
            _village.Villagers.Add(new Villager
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
            _village.Villagers.Remove(villager);
        }

        private Villager RandomVillager()
        {
            var unluckyPerson = Randomizer.Instance.Random.Next(Size);
            return _village.Villagers[unluckyPerson];
        }

        public int Size
        {
            get { return _village.Villagers.Count; }
        }

        public float Suspicion
        {
            get { return _village.Suspicion; }
        }

        public override string Name
        {
            get { return _village.Name; }
        }

        public void IncreaseSuspicion()
        {
            _village.Suspicion = Math.Min(1, _village.Suspicion + (Randomizer.Instance.Roll(15, 5) / 100f)); // Can't get more suspicious than 1
        }

        public void DecreaseSuspicion()
        {
            _village.Suspicion = Math.Max(0, _village.Suspicion - (Randomizer.Instance.Roll(5, 5) / 100f)); // Can't get less suspicious than 0 
        }

        public override bool Upkeep(Game game)
        {
            var somethingHappened = false;
            var heroCount = _heroes.Count;
            for (int i = 0; i < heroCount; i++)
            {
                var hero = _heroes[i];
                hero.MoveToLocation(_object.WorldLocation, _object.RegionLocation);
                var lives = hero.Adventure(game);
                if (!lives)
                {
                    _heroes.Remove(hero);
                    heroCount--;
                }
                else
                {
                    Console.WriteLine($"{hero.Name} is still at large.");
                }

                somethingHappened = true;
            }

            // Roll for adventurer spawn
            // - MAX 3/village
            // - MUST ROLL MORE THAN 16
            // - IF SUCCEEDS CAN ROLL AGAIN -- NOT IMPLEMENTED
            // - only able to spawn after day 5 
            // - have 'pity' counter -- NOT IMPLEMENTED

            if (_heroes.Count < FIGHTER_MAX)
            {
                var newHero = HeroController.TryCreateHero(typeof(Fighter), game, _object.WorldLocation, _object.RegionLocation, false);
                if (newHero != null)
                {
                    _heroes.Add(newHero);

                    Console.WriteLine($"{newHero.Name} rises from {_village.Name} to challenge your power");
                    somethingHappened = true;
                }
            }
            

            return somethingHappened;
        }
    }
}
