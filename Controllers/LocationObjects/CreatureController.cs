using Count.Models;

namespace Count.Controllers
{
    public abstract class CreatureController<T> : LocationObjectController<T> where T : Creature
    {
        public virtual string Name { get { return _object.Name; } }
        public virtual int Hitpoints { get { return _object.Hitpoints; } }
        public virtual int Damage { get { return _object.Damage; } }

        protected CreatureController(Location worldLocation, Location regionLocation): base(worldLocation, regionLocation) {}

        public void MoveToLocation(Location worldLocation, Location regionLocation)
        {
            _object.WorldLocation = worldLocation;
            _object.RegionLocation = regionLocation;
        }
    }
}
