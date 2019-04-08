using System.Collections.Generic;
using System.Linq;
using Count.Controllers.LocationObjects.Structures.Neutral;
using Count.Models;
using Count.Utils;

namespace Count.Controllers
{
    public class WorldController
    {
        private const int WORLD_SIZE = 3; // cant be smaller than inner region // 64 is pretty big

        private World _world { get; set; }
        private StructureController[,] _locationObjects = new StructureController[WORLD_SIZE, WORLD_SIZE];

        public WorldController()
        {
            _world = new World();
            _world.Size = WORLD_SIZE; //WORLD_SIZExWORLD_SIZE
            _world.Day = 1;

            GenerateWorld();
        }

        private void GenerateWorld()
        {
            int totalSize = (WORLD_SIZE * WORLD_SIZE) - 1; // -1 for castle

            // Initialize Villages (Max villages plus a bit, but below cap)
            int villages = 1;
            for (int i = 0; i < villages; i++)
            {
                var villageLocation = GetUnusedWorldLocation();
                var village = new VillageController(villageLocation);
                _locationObjects[villageLocation.X, villageLocation.Y] = village;
            }
            totalSize = totalSize - villages;

            // Initialize Graveyards
            int graveyards = 1;
            for (int i = 0; i < graveyards; i++)
            {
                var graveyardLocation = GetUnusedWorldLocation();
                var graveyard = new GraveyardController(graveyardLocation);
                _locationObjects[graveyardLocation.X, graveyardLocation.Y] = graveyard;
            }
            totalSize = totalSize - graveyards;

            // Initialize Forests
            int forests = totalSize;
            for (int i = 0; i < forests; i++)
            {
                var forestLocation = GetUnusedWorldLocation();
                var forest = new ForestController(forestLocation);
                _locationObjects[forestLocation.X, forestLocation.Y] = forest;
            }
        }

        public Location GetUnusedWorldLocation()
        {
            return GetUnusedWorldLocation(null);
        }

        public Location GetUnusedWorldLocation(List<Location> locations)
        {
            List<Location> locationsShuffled = null;
            if (locations != null)
            {
                Location[] copiedList = new Location[locations.Count];
                locations.CopyTo(copiedList);
                locationsShuffled = copiedList.ToList();
            }
            else
            {
                locationsShuffled = new List<Location>();
                for (int i = 0; i < _locationObjects.GetLength(0); i++)
                {
                    for (int j = 0; j < _locationObjects.GetLength(1); j++)
                    {
                        locationsShuffled.Add(new Location(i, j));
                    }
                }
            }

            locationsShuffled = locationsShuffled.OrderBy(i => Randomizer.Instance.Random.Next()).ToList();
            foreach (var location in locationsShuffled)
            {
                if (GetLocationObjectAtLocation(location) == null)
                    return location;
            }
            return null;
        }

        public StructureController GetLocationObjectAtLocation(Location location)
        {
            return _locationObjects[location.X, location.Y];
        }

        public StructureController AddLocationObject(StructureController locationObject)
        {
            if (GetLocationObjectAtLocation(locationObject.WorldLocation) == null)
            {
                _locationObjects[locationObject.WorldLocation.X, locationObject.WorldLocation.Y] = locationObject;
                return locationObject;
            }
            return null;
        }

        public List<Location> Locations
        {
            get
            {
                var locations = new List<Location>();
                foreach (var locationObject in _locationObjects)
                {
                    locations.Add(locationObject.WorldLocation);
                }
                return locations;
            }
        }

        public StructureController[,] LocationMap
        {
            get { return _locationObjects; }
        }

        public List<StructureController> LocationObjects
        {
            get
            {
                var locationObjects = new List<StructureController>();
                foreach (var locationObject in _locationObjects)
                {
                    locationObjects.Add(locationObject);
                }
                return locationObjects;
            }
        }


        public int Size
        {
            get { return _world.Size; }
        }

        /// <summary>
        /// The day 
        /// </summary>
        public int Day
        {
            get { return _world.Day; }
        }

        /// <summary>
        /// Move to the next day
        /// </summary>
        public void FinishDay()
        {
            _world.Day++;
        }
    }
}
