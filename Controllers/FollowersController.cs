using Count.Models;
using Count.Models.Followers;
using Count.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Count.Controllers
{
    public class FollowersController
    {
        private WorldController _worldController;
        private VampireLordController _vampireLordController;
        private List<Follower> _followers { get; set; }

        public FollowersController(WorldController worldController, VampireLordController vampireLordController)
        {
            _worldController = worldController;
            _vampireLordController = vampireLordController;

            _followers = new List<Follower>();
        }

        public int Total { get { return _followers.Count; } }
        public int GetTotalOfType(Type followerType)
        {
            return _followers.Count(i => i.GetType() == followerType);
        }

        public Type CreateFollower(Villager villager)
        {
            if (Game.IS_DEV)
            {
                Console.WriteLine($"(DEV) VILLAGER STR: {villager.Strength}");
                Console.WriteLine($"(DEV) VILLAGER INT: {villager.Intelligence}");
                Console.WriteLine("");
                Console.WriteLine($"(DEV) ZOMBIE CHANCE: {(float)villager.Strength / (float)(villager.Intelligence + villager.Strength) * 100}");
                Console.WriteLine($"(DEV) VAMPIRE CHANCE: {(float)villager.Intelligence / (float)(villager.Intelligence + villager.Strength) * 100}");
            }

            var rollChance = Randomizer.Instance.Roll(1, villager.Intelligence + villager.Strength);
            if (rollChance <= villager.Intelligence)
            {
                _followers.Add(new Vampire { PreviousLife = villager });
                return typeof(Vampire);
            }
            else
            {
                _followers.Add(new Zombie { PreviousLife = villager });
                return typeof(Zombie);
            }
        }

        public bool TryKillFollower(Type followerType)
        {
            var collection = _followers;
            if (followerType != null)
            {
                collection = collection.Where(i => i.GetType() == followerType).ToList();
            }

            if (collection.Any())
            {
                var unluckyFollowerIndex = Randomizer.Instance.Random.Next(collection.Count);
                var unluckyFollower = collection[unluckyFollowerIndex];

                // remove
                _followers.Remove(unluckyFollower);
                return true;
            }
            return false;
        }
    }
}
