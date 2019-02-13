using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count.Models
{
    public class VampireLord
    {
        public int Hitpoints { get; set; }
        /// <summary>
        /// Last day the vampire was fed
        /// </summary>
        public long LastFed { get; set; }

        /// <summary>
        /// Villagers converted into followers
        /// </summary>
        public int Followers { get; set; }
        /// <summary>
        /// Vampire's hiding spot
        /// </summary>
        public Location Location { get; set; }
    }
}
