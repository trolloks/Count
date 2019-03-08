using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count.Models;

namespace Count.Controllers
{
    public abstract class FriendlyLocationController<T> : LocationObjectController where T : Follower 
    {
        public FriendlyLocationController(Location worldLocation, Location regionLocation) : base(worldLocation, regionLocation)
        {}

        protected List<FollowerController<T>> _followers = new List<FollowerController<T>>();

        /// <summary>
        /// Abstract upkeep method
        /// </summary>
        public abstract bool Upkeep(Models.Game game);

        public ReadOnlyCollection<FollowerController<T>> Followers { get { return _followers.AsReadOnly(); } }

        public void KillFollower(FollowerController<T> follower)
        {
            _followers.Remove(follower);
        }
    }
}

