using System;
using Count.Models;
using Count.Models.Followers;
using Count.Utils;

namespace Count.Controllers
{
    public class FollowerController
    {
        private const int BASE_CONVERT_DC = 5;
        private const int BASE_CHECK_ROLL = 20;

        private Follower _follower { get; set; }

        private FollowerController(Type followerType) {
            if (followerType == typeof(Zombie))
                _follower = new Zombie { Available = true };
            if (followerType == typeof(Vampire))
                _follower = new Vampire { Available = true };
            if (followerType == typeof(Cultist))
                _follower = new Cultist { Available = true };

        }

        /// <summary>
        ///  Checks if you succeed on creating a follower
        /// </summary>
        public static FollowerController TryCreateFollower(Type followerType)
        {
            var convertCheck = true;
            var convertRoll = Randomizer.Instance.Roll(1, BASE_CHECK_ROLL);
            if (Game.IS_DEV)
            {
                Console.WriteLine($"(DEV) CONVERT CHECK: {convertRoll}");
                Console.WriteLine($"(DEV) CONVERT DC CHECK: {BASE_CONVERT_DC}");
            }

            convertCheck = convertRoll >= (BASE_CONVERT_DC);

            if (convertCheck)
            {
                return new FollowerController(followerType);
            }

            return null;
        }

        public Follower Follower { get { return _follower; } }

        //public static FollowerController CreateFollower(Villager villager)
        //{

            //return new FollowerController();

            /*if (Game.IS_DEV)
            {
                Console.WriteLine($"(DEV) VILLAGER STR: {villager.Strength}");
                Console.WriteLine($"(DEV) VILLAGER INT: {villager.Intelligence}");
                Console.WriteLine("");
                Console.WriteLine($"(DEV) ZOMBIE CHANCE: {(float)villager.Strength / (float)(villager.Intelligence + villager.Strength + villager.Charisma) * 100}");
                Console.WriteLine($"(DEV) VAMPIRE CHANCE: {(float)villager.Intelligence / (float)(villager.Intelligence + villager.Strength + villager.Charisma) * 100}");
                Console.WriteLine($"(DEV) CULTIST CHANCE: {(float)villager.Charisma / (float)(villager.Intelligence + villager.Strength + villager.Charisma) * 100}");
            }

            var rollChance = Randomizer.Instance.Roll(1, villager.Intelligence + villager.Strength + villager.Charisma);
            if (rollChance <= villager.Intelligence)
            {
                return new FollowerController(new Vampire { PreviousLife = villager, Available = true });
            }
            else if (rollChance > villager.Intelligence && rollChance <= (villager.Intelligence + villager.Charisma))
            {
                return new FollowerController(new Cultist { PreviousLife = villager, Available = true });
            }
            else
            {
                return new FollowerController(new Zombie { PreviousLife = villager, Available = true });
            }*/
        //}
    }
}
