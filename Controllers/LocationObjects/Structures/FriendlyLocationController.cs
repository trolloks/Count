using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count.Models;

namespace Count.Controllers
{
    public abstract class FriendlyLocationController : StructureController
    {
        public FriendlyLocationController(Location worldLocation) : base(worldLocation)
        {}

        protected List<FollowerController> _followers = new List<FollowerController>();

        public ReadOnlyCollection<FollowerController> Followers { get { return _followers.AsReadOnly(); } }

        public void KillFollower(FollowerController follower)
        {
            _followers.Remove(follower);
        }
    }
}

