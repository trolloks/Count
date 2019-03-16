using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count.Models;

namespace Count.Utils
{
    public static class LocationUtil
    {
        public static string ToStringFromLocations(List<Location> locations)
        {
            var output = "{ ";
            int count = 0;
            foreach (var location in locations)
            {
                if (count > 0)
                    output += ", ";
                output += $"({location.X}, {location.Y})";
                count++;
            }

            output += " }";
            return output;
        }

        public static bool CompareLocations(Location location1, Location location2)
        {
            return location1.X == location2.X && location1.Y == location2.Y;
        }
    }

}
