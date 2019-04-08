namespace Count.Models
{
    public class Plot
    {
        public string Name { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Duration in days
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// Reward in blood
        /// </summary>
        public int Reward { get; set; }
        /// <summary>
        /// Followers required for plot
        /// </summary>
        public int FollowersRequired { get; set; }
        /// <summary>
        /// Minimum level of creature required
        /// </summary>
        public int FollowersMinLevel { get; set; }
    }
}
