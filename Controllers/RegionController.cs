using System.Collections.Generic;
using System.Linq;
using Count.Models;
using Count.Utils;

namespace Count.Controllers
{
    public class RegionController
    {
        private const int REGION_SIZE = 3; 
        ///private const int MAX_VILLAGES = 4;

        private Region Region { get; set; }

        public LocationObjectController[,] LocationObjects = new LocationObjectController[REGION_SIZE, REGION_SIZE];

        public RegionController(Location worldLocation)
        {
            Region = new Region {

                Size = REGION_SIZE,
                Location = worldLocation
            };

            // Initialize Villages
            int villages = 1 + Randomizer.Instance.Roll(1, 2);
            for (int i = 0; i < villages; i++)
            {
                var villageLocation = GetUnusedRegionLocation();
                var village = new VillageController(Region.Location, villageLocation);
                LocationObjects[villageLocation.X, villageLocation.Y] = village;
            }
        }

        public int Size
        {
            get { return Region.Size; }
        }

        public LocationObjectController GetLocationObjectAtLocation(Location location)
        {
            return LocationObjects[location.X, location.Y];
        }

        public T AddLocationObject<T>(T locationObject) where T : LocationObjectController
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
            var locationsShuffled = new List<Location>();
            for (int i = 0; i < LocationObjects.GetLength(0); i++)
            {
                for (int j = 0; j < LocationObjects.GetLength(1); j++)
                {
                    locationsShuffled.Add(new Location(i, j));
                }
            }
            locationsShuffled = locationsShuffled.OrderBy(i => Randomizer.Instance.Random.Next()).ToList();

            foreach(var location in locationsShuffled)
            {
                if (GetLocationObjectAtLocation(location) == null)
                    return location;
            }
            return null;
        }
    }
}
