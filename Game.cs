using Count.Controllers;
using Count.Models;
using Count.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Count
{
    /// <summary>
    /// TODO: 
    /// 1. WIP: Add multiple villages.
    /// 2. Add ability to find additional villages.
    /// 3. Generate village names
    /// 4. Create suspicion decay if village was not visited.
    /// 5. Create multiple follower types.
    /// 6. Concept of mana and spells
    /// 
    /// </summary>
    public class Game
    {
        long _day = 0;
        bool _isNight = true;
        bool _isGameOver = false;

        /// <summary>
        /// You, the vampire lord
        /// </summary>
        VampireLordController _vampire;
        /// <summary>
        /// The world you interact with
        /// </summary>
        WorldController _world;

        public static bool IS_DEV = true;

        public void Start()
        {
            Init();
            Loop();
        }

        private void Init()
        {
            Console.Clear();
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("THE COUNT");
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("One fateful night you are turned into a vampire. You must now use your wits and cunning to survive.");
            Console.WriteLine("");
            Console.WriteLine("Press ENTER to continue");
            Console.ReadLine();

            _world = new WorldController();
            _vampire = new VampireLordController(_world);
        }

        private void Loop()
        {
            while (true)
            {
                // Start of new day
                _day++;

                // Phases
                while(Night());
                while(Day());

                if (_vampire.IsDead)
                {
                    Lose();
                    break;
                }

                if (_isGameOver)
                {
                    Win();
                    break;
                }
            }
        }

        #region "Day"
        private bool Day()
        {
            if (!_vampire.IsDead)
            {
                _isNight = false;

                Console.Clear();
                Console.WriteLine($"~~~ Day {_day} (Day) ~~~");
                Console.WriteLine("");

                var infoText = "The sun rises again and you have to rest. Humanity uses this time to hunt you down";
                Console.WriteLine(infoText);
                Console.WriteLine("");

                // Stat Report
                Console.WriteLine($"HEALTH: {_vampire.Hitpoints}");
                Console.WriteLine($"FOLLOWERS: {_vampire.Followers}");
                Console.WriteLine($"LIVING VILLAGERS: {_world.GetCurrentVillage().Size}");
                Console.WriteLine("");

                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {} // No actions during the day yet. Could be permanent?
            
               
                // Villagers try to find vampire
                if (_world.GetCurrentVillage().Search(_vampire.Location))
                {
                    Console.WriteLine("The villagers have found your hiding place!");
                    if (_vampire.TryKill()) // True if it succeeds
                        Console.WriteLine("With no-one to protect you. You get killed by the villagers!");
                    else
                        Console.WriteLine("A follower gives his life to save yours. Move your hiding place!");
                }
                else
                {
                    Console.WriteLine("The villagers failed to find your hiding place.");
                }

                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                Console.ReadLine();
            }
            return false;
        }
        #endregion
        #region "Night"
        private bool Night()
        {
            if (!_vampire.IsDead)
            {
                _isNight = true;

                Console.Clear();
                Console.WriteLine($"~~~ Day {_day} (Night) ~~~");
                Console.WriteLine("");

                var infoText = "Night falls. You can yet again roam the land.";
                if (_day == 1)
                    infoText += "\nYou rise on your first night as a vampire. You feel a hunger you haven't felt before.\nYou feel compelled to feed on human blood.";
                if (_vampire.DetermineHungerLevel(_day) >= VampireLordController.HUNGER_WARNING_THRESHOLD)
                    infoText += $"\nWarning! You NEED to feed once every {VampireLordController.HUNGER_STARVING_THRESHOLD} days!";
                if (_vampire.DetermineHungerLevel(_day) >= VampireLordController.HUNGER_STARVING_THRESHOLD)
                {
                    infoText += "\nYou are STARVING. You start taking damage, because you are not feeding!";
                    _vampire.Damage(1);
                    if (_vampire.IsDead)
                        return false;
                }

                Console.WriteLine(infoText);
                Console.WriteLine("");

                // Stat Report
                Console.WriteLine($"HEALTH: {_vampire.Hitpoints}");
                Console.WriteLine($"HUNGER LEVEL: {_vampire.DetermineHungerLevel(_day)}");
                Console.WriteLine($"LIVING VILLAGERS: {_world.GetCurrentVillage().Size}");
                if (IS_DEV)
                    Console.WriteLine($"(DEV) VILLAGE SUSPICION: {_world.GetCurrentVillage().Suspicion}");
                Console.WriteLine("");

                // Actions
                Console.WriteLine("1. Hide");
                Console.WriteLine("2. Feed on villager");
                Console.WriteLine("3. Move Lair");

                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {
                    case "3":
                        Console.WriteLine("You spend the night moving to a new location.\nThe village grows more suspicious.");
                        _vampire.MoveLocation();
                        _world.GetCurrentVillage().InvalidateSearch();
                        _world.GetCurrentVillage().IncreaseSuspicion();
                        break;
                    case "2":
                        // try to feed
                        if (_vampire.Feed(_day))
                        {
                            Console.WriteLine("You feed a villager successfully. You hunger recedes. ...For now\nThe village grows more suspicious.");
                            // Effects on village
                            _world.GetCurrentVillage().KillVillager();
                        }
                        else
                            Console.WriteLine("You fail your attempt to feed on a villager. The village grows more suspicious.");
                        _world.GetCurrentVillage().IncreaseSuspicion();
                        break;
                    case "1":
                        Console.WriteLine("You hide for the night. The village grows less suspicious.");
                        _world.GetCurrentVillage().DecreaseSuspicion();
                        break;
                    default:
                        return true;
                       
                }

                // TODO: 
                // Decay villages' suspicion here. EXCEPT the one that was visited (if any)

                // Basic Win Condition - Will change **DEPRECATED
                /*if (_villages[0].Villagers <= 0)
                    _isGameOver = true;*/

                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                Console.ReadLine();
            }
            return false;
        }
        #endregion

        private void Win()
        {
            Console.Clear();
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("GAME OVER");
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("Congratulations! You have been successful with your schemes.");
            Console.WriteLine("");
            Console.WriteLine("Press ENTER to continue");
            Console.ReadLine();
        }

        private void Lose()
        {
            Console.Clear();
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("GAME OVER");
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("You have died! Please try again.");
            Console.WriteLine("");
            Console.WriteLine("Press ENTER to continue");
            Console.ReadLine();
        }
    }
}
