using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count.Models;

namespace Count.Controllers
{
    public abstract class FriendlyLocationController<T, S> : StructureController<T> where S : Follower where T : Structure
    {
        public FriendlyLocationController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {}

        protected List<FollowerController<S>> _followers = new List<FollowerController<S>>();

        /// <summary>
        /// Abstract upkeep method
        /// </summary>
        public abstract bool Upkeep(Game game);

        public ReadOnlyCollection<FollowerController<S>> Followers { get { return _followers.AsReadOnly(); } }

        public void KillFollower(FollowerController<S> follower)
        {
            _followers.Remove(follower);
        }
    }
}

