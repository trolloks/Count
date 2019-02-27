using System;
using System.Collections.Generic;

namespace Count.Models
{
    public class Castle : LocationObject
    {
        /// <summary>
        /// Castle's Total Research
        /// </summary>
        public int ResearchPoints { get; set; }
        /// <summary>
        /// Unlocked Research Items
        /// </summary>
        public List<ResearchItem> UnlockedResearch { get; set; } = new List<ResearchItem>();
    }
}
