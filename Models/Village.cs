using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count.Models
{
    public class Village
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Villager> Villagers { get; set; }
        public float Suspicion { get; set; }
    }
}
