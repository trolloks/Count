namespace Count.Models
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
        /// Last day the vampire was fed
        /// </summary>
        public long LastFed { get; set; }
        /// <summary>
        /// Vampire's hiding spot
        /// </summary>
        public Location Location { get; set; }
    }
}
