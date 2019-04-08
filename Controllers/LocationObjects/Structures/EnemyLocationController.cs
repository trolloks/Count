using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count.Models;

namespace Count.Controllers
{
    public abstract class EnemyLocationController : StructureController
    {
        public EnemyLocationController(Location worldLocation) : base(worldLocation)
        { }

        protected List<HeroController> _heroes = new List<HeroController>();

        public ReadOnlyCollection<HeroController> Heroes { get { return _heroes.AsReadOnly(); } }

        public void KillHero(HeroController hero)
        {
            _heroes.Remove(hero);
        }
    }
}

