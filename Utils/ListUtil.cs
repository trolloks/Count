using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count.Models;

namespace Count.Utils
{
    public static class ListUtil
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
    }

}
