using Count.Models;

namespace Count.Controllers
{
    public abstract class CreatureController : LocationObjectController
    {
        protected Creature _creature { get { return _object as Creature; } }

        public virtual string Name { get { return _creature.Name; } }
        public virtual int Hitpoints { get { return _creature.Hitpoints; } }
        public virtual int Attack { get { return _creature.Attack; } }

        protected CreatureController(Location worldLocation, Location regionLocation): base(worldLocation, regionLocation) {}

        public void MoveToLocation(Location worldLocation, Location regionLocation)
        {
            _object.WorldLocation = worldLocation;
            _object.RegionLocation = regionLocation;
        }

        /// <summary>
        /// Initiate an attack on another creature
        /// </summary>
        /// <param name="otherCreature"></param>
        public static Creature Fight(Creature thisCreature, Creature otherCreature)
        {
            while (thisCreature.Hitpoints > 0 && otherCreature.Hitpoints > 0)
            {
                // damage other creature first
                Damage(otherCreature, thisCreature.Attack);
                // other creatures turn now
                Damage(thisCreature, otherCreature.Attack);
            }

            if (thisCreature.Hitpoints <= 0 && otherCreature.Hitpoints > 0)
                return otherCreature;
            else if (otherCreature.Hitpoints <= 0 && thisCreature.Hitpoints > 0)
                return thisCreature;
            else
                return null;

        }


        public static void Damage(Creature creature, int damage)
        {
            creature.Hitpoints -= damage;
        }
    }
}
