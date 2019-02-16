using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count.Models
{
    public class Village
    {
        public string Name { get; set; }
        public List<Villager> Villagers { get; set; }
        public float Suspicion { get; set; }
        public List<Location> LocationsSearched { get; set; } = new List<Location>();
        public bool IsLocationFound { get; set; }
    }
}
