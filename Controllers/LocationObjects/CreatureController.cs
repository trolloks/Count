using Count.Models;

namespace Count.Controllers
{
    public abstract class CreatureController : LocationObjectController
    {
        protected Creature _creature { get { return _object as Creature; } }

        public virtual string Name { get { return _creature.Name; } }
        public virtual int Hitpoints { get { return _creature.Hitpoints; } }
        public virtual int Damage { get { return _creature.Damage; } }

        protected CreatureController(Location worldLocation, Location regionLocation): base(worldLocation, regionLocation) {}

        public void MoveToLocation(Location worldLocation, Location regionLocation)
        {
            _object.WorldLocation = worldLocation;
            _object.RegionLocation = regionLocation;
        }
    }
}
