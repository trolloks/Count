using System;
using System.Collections.Generic;

namespace Count.Models
{
    public class Village : Structure
    {
        public Guid Id { get; set; }
        public List<Villager> Villagers { get; set; }
        public float Suspicion { get; set; }
    }
}
