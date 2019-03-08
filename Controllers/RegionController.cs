using System.Collections.Generic;
using System.Linq;
using Count.Models;
using Count.Utils;

namespace Count.Controllers
{
    public class RegionController
    {
        private const int REGION_SIZE = 64;
        private const int INNER_REGION_SIZE = 4;
        ///private const int MAX_VILLAGES = 4;

        private Region _region { get; set; }

        public StructureController[,] LocationObjects = new StructureController[REGION_SIZE, REGION_SIZE];

        public RegionController(Location worldLocation)
        {
            _region = new Region
            {

                Size = REGION_SIZE,
                Location = worldLocation
            };

            GenerateVillages();
        }

        private void GenerateVillages()
        {
            for (int k = 0; k < (REGION_SIZE / INNER_REGION_SIZE); k++)
            {
                int xOffset = INNER_REGION_SIZE * k;
                for (int l = 0; l < (REGION_SIZE / INNER_REGION_SIZE); l++)
                {
                    int yOffset = INNER_REGION_SIZE * l;
                    var locations = new List<Location>();

                    for (int x = xOffset; x < (xOffset + INNER_REGION_SIZE); x++)
                    {
                        for (int y = yOffset; y < (yOffset + INNER_REGION_SIZE); y++)
                        {
                            locations.Add(new Location(x, y));
                        }
                    }

                    // Initialize Villages
                    int villages = 3 + Randomizer.Instance.Roll(2, 3);
                    for (int i = 0; i < villages; i++)
                    {
                        var villageLocation = GetUnusedRegionLocation(locations);
                        var village = new VillageController(_region.Location, villageLocation);
                        LocationObjects[villageLocation.X, villageLocation.Y] = village;
                    }
                }
            }
        }

        public int Size
        {
            get { return _region.Size; }
        }

        public StructureController GetLocationObjectAtLocation(Location location)
        {
            return LocationObjects[location.X, location.Y];
        }

        public T AddLocationObject<T>(T locationObject) where T : StructureController
        {
            if (GetLocationObjectAtLocation(locationObject.RegionLocation) == null)
            {
                LocationObjects[locationObject.RegionLocation.X, locationObject.RegionLocation.Y] = locationObject;
                return locationObject;
            }
            return null;
        }

        public Location GetUnusedRegionLocation()
        {
            return GetUnusedRegionLocation(null);
        }

        public Location GetUnusedRegionLocation(List<Location> locations)
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
                for (int i = 0; i < LocationObjects.GetLength(0); i++)
                {
                    for (int j = 0; j < LocationObjects.GetLength(1); j++)
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
    }
}
