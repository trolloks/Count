using Count.Controllers;
using Count.Models;
using Count.Models.Followers;
using Count.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Count
{
    /// <summary>
    /// TODO: 
    /// 
    /// 1. Break up world into segments
    /// 2. Add concept of distance.
    /// 3. X Amount of villages into section(region?), with possible special locations
    /// 4. Concept of mana and spells
    /// 5. Progression -> Castle upgrades etc?
    /// 6. Generate village/villager/region names
    /// 
    /// 4** Create stats for villagers. Each stat corresponds to the type of follower that could be created by feeding. 
    /// ie. Strong villager could become strong follower. Smart villager, smart follower.etc.
    /// 
    /// </summary>
    public class Game
    {
        bool _isGameOver = false;

        /// <summary>
        /// You, the vampire lord
        /// </summary>
        VampireLordController _vampire;
        /// <summary>
        /// The world you interact with
        /// </summary>
        WorldController _world;

        public static bool IS_DEV = false;

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

                // End the day
                _world.FinishDay();
            }
        }

        #region "Day"
        private bool Day()
        {
            if (!_vampire.IsDead)
            {
                Console.Clear();
                Console.WriteLine($"~~~ Day {_world.Day} (Day) ~~~");
                Console.WriteLine("");

                var infoText = "The sun rises again and you have to rest. Humanity uses this time to hunt you down";
                Console.WriteLine(infoText);
                Console.WriteLine("");

                // Stat Report
                PrintStats();
                Console.WriteLine("");

                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {} // No actions during the day yet. Could be permanent?
               
                // Villagers try to find vampire
                if (_world.Search(_vampire.Location))
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

                // You sleep during the day
                _vampire.Sleep();

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
                var infoText = "Night falls. You can yet again roam the land.";
               
                if (_vampire.DetermineHungerLevel() >= VampireLordController.HUNGER_WARNING_THRESHOLD)
                    infoText += $"\nWarning! You NEED to feed once every {VampireLordController.HUNGER_STARVING_THRESHOLD} days!";
                if (_vampire.DetermineHungerLevel() >= VampireLordController.HUNGER_STARVING_THRESHOLD)
                {
                    infoText += "\nYou are STARVING. You start taking damage, because you are not feeding!";
                    _vampire.Damage(1);
                    if (_vampire.IsDead)
                        return false;
                }
                
                while (_vampire.ActionPoints > 0)
                {
                    Console.Clear();
                    Console.WriteLine($"~~~ Day {_world.Day} (Night) ~~~");
                    Console.WriteLine("");
                    Console.WriteLine(infoText);
                    Console.WriteLine("");
                    Console.WriteLine($"***{_vampire.ActionPoints} ACTION{(_vampire.ActionPoints > 1 ? "S" : "")} AVAILABLE****");
                    Console.WriteLine("");

                    // Stat Report
                    PrintStats();

                    Console.WriteLine("");

                    // Actions
                    Console.WriteLine("1. Venture into the world");
                    Console.WriteLine("2. Move Lair (1 Action)");

                    Console.WriteLine("");
                    Console.Write(": ");

                    var option = Console.ReadLine();
                    Console.Clear();
                    switch (option)
                    {
                        case "1":
                            EnterVillage();
                            break;
                        case "2":
                            Console.WriteLine("You spend the night moving to a new location.");
                            _vampire.MoveLocation();
                            _world.InvalidateSearch();
                            // Exert after an action
                            _vampire.Exert(1);
                            break;
                    }

                    Console.WriteLine("");
                    Console.WriteLine("Press ENTER to continue");
                    Console.ReadLine();
                }

                // Decay villages' suspicion here. EXCEPT the one that was visited (if any)
                foreach (var region in _world.Regions)
                {
                    foreach (var village in region.Villages)
                    {
                        if (!village.Equals(region.GetCurrentVillage()))
                            village.DecreaseSuspicion();
                    }
                }
                
               

                // Basic Win Condition - Will change **DEPRECATED
                /*if (_villages[0].Villagers <= 0)
                    _isGameOver = true;*/
            }
            return false;
        }

        private void EnterVillage()
        {
            var finishedEnterVillage = false;
            while (!finishedEnterVillage && _vampire.ActionPoints > 0)
            {
                Console.Clear();
                Console.WriteLine($"Welcome to {_world.GetCurrentRegion().GetCurrentVillage().Name}");
                Console.WriteLine($"Population: {_world.GetCurrentRegion().GetCurrentVillage().Size}");
                Console.WriteLine("----------------------------------------------------------------------------");
                PrintStats();
                if (IS_DEV)
                    Console.WriteLine($"(DEV) VILLAGE SUSPICION: {_world.GetCurrentRegion().GetCurrentVillage().Suspicion}");
                if (_world.GetCurrentRegion().GetCurrentVillage().Suspicion >= VillageController.SUSPICION_WARNING_THRESHOLD)
                {
                    Console.WriteLine("**WARNING** The village has been alerted to you presence. It might be better to seek a different village");
                }
                Console.WriteLine("");
                Console.WriteLine($"***{_vampire.ActionPoints} ACTION{(_vampire.ActionPoints > 1 ? "S" : "")} AVAILABLE****");
                Console.WriteLine("");
                Console.WriteLine("1. Feed! (1 Action)");
                Console.WriteLine($"\t- Feed to sate your hunger. If you don't feed once every {VampireLordController.HUNGER_STARVING_THRESHOLD} days you will start taking damage.");
                Console.WriteLine("2. Convert Villager into follower (1 Action)");
                Console.WriteLine("\t- Followers help you further your schemes. They would even give their lives to protect you against the villagers.");
                Console.WriteLine("3. Choose another village");
                Console.WriteLine("q. Go back to previous menu");
                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {
                    case "1":
                        // try to feed
                        if (_vampire.Feed())
                        {
                            Console.WriteLine("You feed a villager successfully. You hunger recedes. ...For now\nThe village grows more suspicious.");
                            // Effects on village
                            _world.GetCurrentRegion().GetCurrentVillage().KillVillager();
                        }
                        else
                            Console.WriteLine("You fail your attempt to feed on a villager. The village grows more suspicious.");
                        _world.GetCurrentRegion().GetCurrentVillage().IncreaseSuspicion();
                        // Exert after an action
                        _vampire.Exert(1);
                        finishedEnterVillage = true;
                        break;
                    case "2":
                        // try to convert
                        var followerConvertedType = _vampire.TryConvertFollower();
                        if (followerConvertedType != null)
                        {
                            if (followerConvertedType == typeof(Zombie))
                                Console.WriteLine("You have converted a villager into a zombie. This follower would give his life for you.");
                            if (followerConvertedType == typeof(Vampire))
                                Console.WriteLine("You have converted a villager into a lesser vampire. This follower will spread your evil.");
                        }
                        else
                            Console.WriteLine("You fail your attempt to convert a villager. The village grows more suspicious.");
                        _world.GetCurrentRegion().GetCurrentVillage().IncreaseSuspicion();
                        // Exert after an action
                        _vampire.Exert(1);
                        finishedEnterVillage = true;
                        break;
                    case "3":
                        ChooseVillage();
                        break;
                    case "q":
                        finishedEnterVillage = true;
                        break;

                }
            }
        }

        private void ChooseVillage()
        {
            var finishedChooseVillage = false;
            while (!finishedChooseVillage && _vampire.ActionPoints > 0)
            {
                Console.Clear();
                Console.WriteLine("Currently you know about the following villages in the area:");
                var index = 0;
                foreach (var village in _world.GetCurrentRegion().Villages)
                {
                    Console.WriteLine($"{++index}. {village.Name}{(village.Suspicion >= VillageController.SUSPICION_WARNING_THRESHOLD ? " (Alerted)":"")}");
                }

                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("");
                Console.WriteLine($"***{_vampire.ActionPoints} ACTION{(_vampire.ActionPoints > 1 ? "S" : "")} AVAILABLE****");
                Console.WriteLine("");
                Console.WriteLine("Options:");
                Console.WriteLine("");
                Console.WriteLine("1-x. Enter Village");
                // Only show another village option if all previous villages suspicion is above 80%
                if (!_world.GetCurrentRegion().Villages.Any(i => i.Suspicion < VillageController.SUSPICION_WARNING_THRESHOLD))
                    Console.WriteLine("s. Search for another village (1 Action)");
                Console.WriteLine("q. Go back to previous menu");
                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();
                int intOption = -1;
                var couldParse = int.TryParse(option, out intOption);
                if (couldParse && intOption > 0 && (intOption - 1) < _world.GetCurrentRegion().Villages.Count)
                {
                    _world.GetCurrentRegion().SetCurrentVillage(_world.GetCurrentRegion().Villages[intOption - 1]); // minus one for index;
                    finishedChooseVillage = true;

                    Console.WriteLine($"...Now entering {_world.GetCurrentRegion().GetCurrentVillage().Name}...");
                    Console.WriteLine("");
                    Console.WriteLine("Press ENTER to continue");
                    Console.ReadLine();
                    finishedChooseVillage = true;
                }
                else if (option == "s" && (!_world.GetCurrentRegion().Villages.Any(i => i.Suspicion < VillageController.SUSPICION_WARNING_THRESHOLD)))
                {
                    var newVillage =_world.GetCurrentRegion().AddVillage();
                    Console.WriteLine($"By searching you find another village -> {newVillage.Name}");
                    _world.GetCurrentRegion().SetCurrentVillage(newVillage);

                    // Exert after an action
                    _vampire.Exert(1);
                    finishedChooseVillage = true;
                }
                else if (option == "q")
                {
                    finishedChooseVillage = true;
                }
            }

        }
        #endregion

        private void PrintStats()
        {
            Console.WriteLine($"HEALTH: {_vampire.Hitpoints}");
            Console.WriteLine($"HUNGER LEVEL: {_vampire.DetermineHungerLevel()}");
            Console.WriteLine($"FOLLOWERS: {_vampire.GetFollowers().Total}");
            if (_vampire.GetFollowers().GetTotalOfType(typeof(Vampire)) > 0)
                Console.WriteLine($"- VAMPIRES: {_vampire.GetFollowers().GetTotalOfType(typeof(Vampire))}");
            if (_vampire.GetFollowers().GetTotalOfType(typeof(Zombie)) > 0)
                Console.WriteLine($"- ZOMBIES: {_vampire.GetFollowers().GetTotalOfType(typeof(Zombie))}");
        }

        private void Win()
        {
            Console.Clear();
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("GAME OVER");
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("Congratulations! You have been successful with your schemes.");
            Console.WriteLine("");
        }

        private void Lose()
        {
            Console.Clear();
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("GAME OVER");
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("You have died! Please try again.");
            Console.WriteLine("");
        }
    }
}
