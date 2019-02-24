using System;
using Count.Models;
using Count.Models.Followers;
using Count.Utils;

namespace Count.Controllers
{
    public class FollowerController
    {
        public Follower Follower { get; set; }
        private FollowerController(Follower follower) {
            Follower = follower;
        }

        public static FollowerController CreateFollower(Villager villager)
        {
            if (Game.IS_DEV)
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
            }
        }
    }
}
