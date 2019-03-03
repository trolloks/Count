using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count.Models;

namespace Count.Controllers
{
    public abstract class LocationObjectController
    {
        public Location WorldLocation { get; set; }
        public Location RegionLocation { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; }
        protected List<FollowerController> _followers = new List<FollowerController>();

        /// <summary>
        /// Abstract upkeep method
        /// </summary>
        public abstract void Upkeep(Models.Game game);

        public LocationObjectController(Location worldLocation, Location regionLocation) {
            WorldLocation = worldLocation;
            RegionLocation = regionLocation;
        }

        public ReadOnlyCollection<FollowerController> Followers { get { return _followers.AsReadOnly(); } }

        public void KillFollower(FollowerController follower)
        {
            _followers.Remove(follower);
        }
    }
}
