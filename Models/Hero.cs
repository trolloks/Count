namespace Count.Models
{
    public class Hero
    {
        public int Hitpoints { get; set; }
        public string Name { get; set; }
        public Location WorldLocation { get; set; }
        public Location RegionLocation { get; set; }
    }
}
