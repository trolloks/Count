namespace Count.Models
{
    public class Vampire
    {
        /// <summary>
        /// Amount of actions able to take in one night
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
        /// Villagers converted into followers
        /// </summary>
        public int Followers { get; set; }
    }
}
