using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count.Models;

namespace Count.Controllers
{
    public abstract class FriendlyLocationController : StructureController
    {
        public FriendlyLocationController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {}

        protected List<FollowerController> _followers = new List<FollowerController>();

        /// <summary>
        /// Abstract upkeep method
        /// </summary>
        public abstract bool Upkeep(Game game);

        public ReadOnlyCollection<FollowerController> Followers { get { return _followers.AsReadOnly(); } }

        public void KillFollower(FollowerController follower)
        {
            _followers.Remove(follower);
        }
    }
}

