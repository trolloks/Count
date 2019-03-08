using System;
using System.Collections.Generic;
using System.Linq;
using Count.Models;
using Count.Utils;

namespace Count.Controllers
{
    public class WorldController
    {
        private const int WORLD_SIZE = 10;
        private World World { get; set; }
        private List<HeroController<Fighter>> _heroes;

        private readonly RegionController[,] _regions;

        public WorldController()
        {
            World = new World();
            World.Size = WORLD_SIZE; //WORLD_SIZExWORLD_SIZE
            World.Day = 1;

            _regions = new RegionController[WORLD_SIZE, WORLD_SIZE];
            _heroes = new List<HeroController<Fighter>>();
        }

        public int Size
        {
            get { return World.Size; }
        }

        public RegionController GetRegion(Location location)
        {
            return _regions[location.X, location.Y];
        }

        public RegionController AddRegionAtLocation(Location location)
        {
            if (GetRegion(location) == null)
            {
                var region = new RegionController(location);
                _regions[location.X, location.Y] = region;
                return region;
            }
            return null;
        }

        public Location GetUnusedWorldLocation()
        {
            var locationsShuffled = new List<Location>();
            for (int i = 0; i < _regions.GetLength(0); i++)
            {
                for (int j = 0; j < _regions.GetLength(1); j++)
                {
                    locationsShuffled.Add(new Location(i, j));
                }
            }
            locationsShuffled = locationsShuffled.OrderBy(i => Randomizer.Instance.Random.Next()).ToList();

            foreach (var location in locationsShuffled)
            {
                if (GetRegion(location) == null)
                    return location;
            }
            return null;
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

        private const int BASE_SPAWN_MAX_HERO = 5;
        private const int BASE_SPAWN_PITY = 5;

        private int _pityCounter = 0;

        public bool Upkeep(Models.Game game)
        {
            var somethingHappened = false;
            var heroCount = _heroes.Count;
            for (int i = 0; i < heroCount; i++)
            {
                var hero = _heroes[i];
                Location location = game.KnownLocations.OrderBy(j => Randomizer.Instance.Random.Next()).FirstOrDefault();
                hero.MoveToLocation(hero.WorldLocation, location);
                var lives = hero.Hero(game);
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
            // - MAX 5 -- NOT IMPLEMENTED
            // - MUST ROLL MORE THAN 16
            // - IF SUCCEEDS CAN ROLL AGAIN -- NOT IMPLEMENTED
            // - only able to spawn after day 5 
            // - have 'pity' counter -- NOT IMPLEMENTED
            var village = game.KnownVillages.OrderBy(i => Randomizer.Instance.Random.Next()).FirstOrDefault();
            if (village != null)
            {
                var hero = HeroController<Fighter>.TryCreateHero(game, village.WorldLocation, village.RegionLocation, false);
                if (hero != null)
                {
                    _heroes.Add(hero);

                    Console.WriteLine($"{hero.Name} rises from {village.Name} to challenge your power");
                    somethingHappened = true;
                }
            }

            return somethingHappened;
        }

        /// <summary>
        /// Try to find the vampire 
        /// </summary>
        /// <param name="vampireLocation">The "unknown" location of the vampired being searched</param>
        /// <returns>True, if vampire location is found</returns>
       /* public bool Search(Location vampireLocation)
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
        */
    }
}
