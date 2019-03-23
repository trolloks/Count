using System;
using System.Collections.Generic;
using System.Linq;
using Count.Models;
using Count.Utils;

namespace Count.Controllers
{
    public class VillageController : EnemyLocationController
    {
        private Village _village { get { return _object as Village; } }

        protected static int _globalVillageCount;
        private const int FIGHTER_MAX = 3;
        private int _pendingCorpses;

        // temp (For names)
        int villagerCounter = 1;

        public VillageController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {
            // Create village
            _object = new Village()
            {
                Id = Guid.NewGuid(),
                Name = $"Village-{++_globalVillageCount}", // temp
                Villagers = new List<Villager>(),
                WorldLocation = worldLocation,
                RegionLocation = regionLocation
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

        public bool TryKillVillager()
        {
            if (Size > 0)
            {
                KillVillager(RandomVillager());
                return true;
            }

            return false;
        }

        private void KillVillager(Villager villager)
        {
            _village.Villagers.Remove(villager);
            _pendingCorpses++;
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

        public override string Name
        {
            get { return Size > 0 ? _village.Name : $"{_village.Name} (Empty)"; }
        }

        public override bool Upkeep(Game game)
        {
            var somethingHappened = false;
            var heroCount = _heroes.Count;
            for (int i = 0; i < heroCount; i++)
            {
                var hero = _heroes[i];

                Location location = game.KnownLocations.OrderBy(j => Randomizer.Instance.Random.Next()).FirstOrDefault();
                hero.MoveToLocation(_object.WorldLocation, location);

                var lives = hero.Adventure(game);
                if (!lives)
                {
                    _heroes.Remove(hero);
                    heroCount--;
                }
                else if (!game.VampireLord.IsDead)
                {
                    Console.WriteLine($"{hero.Name} is still alive!");
                }
                else
                {
                    // You dead!
                    return true;
                }

                somethingHappened = true;
            }

            // Roll for adventurer spawn
            // - MAX 3/village
            // - MUST ROLL MORE THAN 17
            // - IF SUCCEEDS CAN ROLL AGAIN -- NOT IMPLEMENTED
            // - only able to spawn after day 5 
            // - have 'pity' counter -- NOT IMPLEMENTED

            if (_heroes.Count < FIGHTER_MAX && Size > 0)
            {
                var newHero = HeroController.TryCreateHero(typeof(Fighter), game, _object.WorldLocation, _object.RegionLocation, false);
                if (newHero != null)
                {
                    _heroes.Add(newHero);
                    TryKillVillager(); // reduce villager size

                    Console.WriteLine($"{newHero.Name} rises from {_village.Name} to challenge your power");
                    somethingHappened = true;
                }
            }

            // Move corpses into global pool
            game.VampireLord.IncreaseCorpses(_pendingCorpses);
            // Reset corpse count
            _pendingCorpses = 0;

            return somethingHappened;
        }
    }
}
