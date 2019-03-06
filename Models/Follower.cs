namespace Count.Models
{
    public class Follower 
    {
        public bool Available { get; set; }
        public int Damage { get; set; }
        public Location WorldLocation { get; set; }
        public Location RegionLocation { get; set; }
    }
}
