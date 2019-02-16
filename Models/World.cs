using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count.Models
{
    public class World
    {
        public int Size { get; set; } 
        public int Day { get; set; }
        public List<Location> VampireLocationsSearched { get; set; } = new List<Location>();
        public bool IsVampireLocationFound { get; set; }
    }
}
