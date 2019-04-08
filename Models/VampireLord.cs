﻿namespace Count.Models
{
    public class VampireLord
    {
        /// <summary>
        /// Amount of actions able to take in one night
        /// </summary>
        public int ActionPointsMax { get; set; }
        /// <summary>
        /// Amount of actions able to take currently
        /// </summary>
        public int ActionPoints { get; set; }
        /// <summary>
        /// Vampire's total hitpoints
        /// </summary>
        public int Hitpoints { get; set; }
        /// <summary>
        /// Vampire's total blood
        /// </summary>
        public int Blood { get; set; }
        /// <summary>
        /// Vampire's total corpses
        /// </summary>
        public int Corpses { get; set; }
        /// <summary>
        /// Vampire's location
        /// </summary>
        public Location WorldLocation { get; set; }
    }
}
