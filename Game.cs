using Count.Controllers;
using Count.Models;
using Count.Models.Followers;
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
        /// Your castle
        /// </summary>
        CastleController _castle;
        /// <summary>
        /// The world you interact with
        /// </summary>
        WorldController _world;

        Location _startingWorldLocation;
        Location _startingRegionLocation;

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

            // Get starting World location
            _startingWorldLocation = _world.GetUnusedWorldLocation();
            // Generate starting Region
            var startingRegion = _world.AddRegionAtLocation(_startingWorldLocation);
            // Get starting Region location
            _startingRegionLocation = startingRegion.GetUnusedRegionLocation();
            // Create castle
            _castle = startingRegion.AddLocationObject(new CastleController(_startingWorldLocation, _startingRegionLocation));
            // Create vampire
            _vampire = new VampireLordController(_world, _castle, _startingWorldLocation, _startingRegionLocation);
            
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
                /*if (_world.Search(_vampire.Location))
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
                }*/

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
                var infoText = "Night falls. You awake from your daily slumber within the walls of your castle. What would you like to do tonight?";
               
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
                    Console.WriteLine("----------------------------------------------------------------------------");
                    Console.WriteLine(infoText);
                    Console.WriteLine("----------------------------------------------------------------------------");
                    Console.WriteLine("");

                    // Stat Report
                    PrintStats();
                    Console.WriteLine("");

                    Console.WriteLine($"***{_vampire.ActionPoints} ACTION{(_vampire.ActionPoints > 1 ? "S" : "")} AVAILABLE****");

                    Console.WriteLine("");
                    Console.WriteLine("----------------------------------------------------------------------------");
                    Console.WriteLine("Actions: ");
                    Console.WriteLine("");

                    // Actions
                    Console.WriteLine("1. See the map");
                    /*Console.WriteLine("2. Move Lair (1 Action)");
                    Console.WriteLine("\t- Moving your lair, forces the villagers to restart their search for your location.");*/

                    Console.WriteLine("");
                    Console.Write(": ");

                    var option = Console.ReadLine();
                    Console.Clear();
                    switch (option)
                    {
                        case "1":
                            EnterRegion();
                            break;
                        /*case "2":
                            Console.WriteLine("You spend the night moving to a new location.");
                            _vampire.MoveLocation();
                            _world.InvalidateSearch();
                            // Exert after an action
                            _vampire.Exert(1);
                            break;*/
                    }

                    Console.WriteLine("");
                    Console.WriteLine("Press ENTER to continue");
                    Console.ReadLine();
                }

                // Decay villages' suspicion here. EXCEPT the one that was visited (if any)
                /*foreach (var region in _world.Regions)
                {
                    foreach (var village in region.Villages)
                    {
                        if (!village.Equals(region.GetCurrentVillage()))
                            village.DecreaseSuspicion();
                    }
                }*/

                // Basic Win Condition - Will change **DEPRECATED
                /*if (_villages[0].Villagers <= 0)
                    _isGameOver = true;*/
            }
            return false;
        }

        private void EnterRegion()
        {
            var finishedEnterRegion = false;
            while (!finishedEnterRegion && _vampire.ActionPoints > 0)
            {
                Console.Clear();
                Console.WriteLine($"~~~ Day {_world.Day} (Night) ~~~");
                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("Current Location: (L)");
                var region = _world.GetRegion(_vampire.WorldLocation);
                var currentLocationObject = region.GetLocationObjectAtLocation(_vampire.RegionLocation);
                if (currentLocationObject == null)
                    Console.Write("In the middle of nowhere");
                else if (currentLocationObject.GetType() == typeof(CastleController))
                {
                    var currentCastle = currentLocationObject as CastleController;
                    Console.WriteLine("Your Castle");
                }
                else if (currentLocationObject.GetType() == typeof(VillageController))
                {
                    var currentVillage = currentLocationObject as VillageController;
                    Console.WriteLine(currentVillage.Name + (currentVillage.Suspicion >= VillageController.SUSPICION_WARNING_THRESHOLD ? "(Alerted)" : ""));
                }

                Console.WriteLine("");
                Console.WriteLine("Map:");
                
                var pointsOfInterest = new List<LocationObjectController>();
                int poi = 1;
                for (int i = 0; i < region.LocationObjects.GetLength(0); i++)
                {
                    for (int j = 0; j < region.LocationObjects.GetLength(1); j++)
                    {
                        var currentLocation = new Location(i, j);
                        if (_vampire.RegionLocation.X == i && _vampire.RegionLocation.Y == j)
                        {
                            Console.Write("L");
                            continue;
                        }

                        var locationObject = region.GetLocationObjectAtLocation(currentLocation);
                        if (locationObject == null)
                            Console.Write("-");
                        else 
                        {
                            pointsOfInterest.Add(locationObject);
                            Console.Write($"{poi++}");
                        }
                    }
                    Console.WriteLine("");
                }
                Console.WriteLine("");
                Console.WriteLine("Points of interest:");
                foreach (var pointOfInterest in pointsOfInterest)
                {

                    Console.Write($"{pointsOfInterest.IndexOf(pointOfInterest) + 1}. {pointOfInterest.Name}");
                    if (pointOfInterest.GetType() == typeof(VillageController))
                        Console.Write($"{((pointOfInterest as VillageController).Suspicion >= VillageController.SUSPICION_WARNING_THRESHOLD ? "(Alerted)" : "")}");

                    Console.WriteLine("");
                }

                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("");
                PrintStats();
                Console.WriteLine("");
                Console.WriteLine($"***{_vampire.ActionPoints} ACTION{(_vampire.ActionPoints > 1 ? "S" : "")} AVAILABLE****");
                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------------------");

                Console.WriteLine("Actions: ");
                Console.WriteLine("");

                // Actions
                Console.WriteLine($"{1}-{pointsOfInterest.Count}. Go to point of interest");
                Console.WriteLine($"E. Enter current location");
                if (currentLocationObject.GetType() == typeof(VillageController))
                    Console.WriteLine($"C. Send Cultist to \"convince\" village (1 Action)");

                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();

                int poiOption = -1;
                if (int.TryParse(option, out poiOption))
                {
                    _vampire.Move(pointsOfInterest[poiOption - 1].WorldLocation, pointsOfInterest[poiOption - 1].RegionLocation);
                }
                else if (option == "E")
                {
                    // Enter location
                    if (currentLocationObject.GetType() == typeof(VillageController))
                    {
                        var village = currentLocationObject as VillageController;
                        if (village.Suspicion >= VillageController.SUSPICION_WARNING_THRESHOLD)
                        {
                            Console.Clear();
                            Console.WriteLine("The village has been alerted to you presence. It might be better to seek a different village");
                            Console.WriteLine("");
                            Console.WriteLine("Press ENTER to continue");
                            Console.ReadLine();
                        }  
                        else
                            EnterVillage(village);
                    }
                       
                    if (currentLocationObject.GetType() == typeof(CastleController))
                        finishedEnterRegion = true;
                }
                else if (option == "C")
                {
                    if (currentLocationObject.GetType() == typeof(VillageController))
                    {
                        var village = currentLocationObject as VillageController;
                        var succeeded = _vampire.Followers.Any(i => i.Follower.GetType() == typeof(Cultist));
                        if (succeeded)
                        {
                            village.DecreaseSuspicion();
                            _vampire.Exert(1);
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("You don't have any cultists");
                            Console.WriteLine("");
                            Console.WriteLine("Press ENTER to continue");
                            Console.ReadLine();
                        }
                    }
                }
                else if (option == "Q")
                {
                    finishedEnterRegion = true;
                }
            }
        }

        
        private void EnterVillage(VillageController village)
        {
            var finishedEnterVillage = false;
            while (!finishedEnterVillage && _vampire.ActionPoints > 0)
            {
                Console.Clear();
                Console.WriteLine($"~~~ Day {_world.Day} (Night) ~~~");
                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine($"Welcome to {village.Name}");
                Console.WriteLine($"Population: {village.Size}");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("");
                PrintStats();
                if (IS_DEV)
                    Console.WriteLine($"(DEV) VILLAGE SUSPICION: {village.Suspicion}");
                Console.WriteLine("");
                Console.WriteLine($"***{_vampire.ActionPoints} ACTION{(_vampire.ActionPoints > 1 ? "S" : "")} AVAILABLE****");
                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("Actions: ");
                Console.WriteLine("");

                Console.WriteLine("1. Feed! (1 Action)");
                Console.WriteLine($"\t- Feed to sate your hunger. If you don't feed once every {VampireLordController.HUNGER_STARVING_THRESHOLD} days you will start taking damage.");
                Console.WriteLine("2. Convert Villager into follower (1 Action)");
                Console.WriteLine("\t- Followers help you further your schemes. They would even give their lives to protect you against the villagers.");
                Console.WriteLine("Q. Go back to previous menu");
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
                            village.KillVillager();
                        }
                        else
                            Console.WriteLine("You fail your attempt to feed on a villager. The village grows more suspicious.");
                        village.IncreaseSuspicion();
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
                            if (followerConvertedType == typeof(Cultist))
                                Console.WriteLine("You have converted a villager into a cultist. This follower will change the thoughts of the villagers.");
                            if (followerConvertedType == typeof(Vampire))
                                Console.WriteLine("You have converted a villager into a lesser vampire. This follower will spread your evil.");
                        }
                        else
                            Console.WriteLine("You fail your attempt to convert a villager. The village grows more suspicious.");
                        village.IncreaseSuspicion();
                        // Exert after an action
                        _vampire.Exert(1);
                        finishedEnterVillage = true;
                        break;
                    case "Q":
                        finishedEnterVillage = true;
                        break;

                }
            }
        }

        /*
        private void ChooseVillage()
        {
            var finishedChooseVillage = false;
            while (!finishedChooseVillage && _vampire.ActionPoints > 0)
            {
                Console.Clear();
                Console.WriteLine($"~~~ Day {_world.Day} (Night) ~~~");
                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("Currently you know about the following villages in the area:");
                var index = 0;
                foreach (var village in _world.GetCurrentRegion().Villages)
                {
                    Console.WriteLine($"{++index}. {village.Name}{(village.Suspicion >= VillageController.SUSPICION_WARNING_THRESHOLD ? " (Alerted)":"")}");
                }

                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("");
                PrintStats();
                Console.WriteLine("");
                Console.WriteLine($"***{_vampire.ActionPoints} ACTION{(_vampire.ActionPoints > 1 ? "S" : "")} AVAILABLE****");
                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("Actions: ");
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

    */
        #endregion

        private void PrintStats()
        {
            Console.WriteLine($"HEALTH: {_vampire.Hitpoints}");
            Console.WriteLine($"HUNGER LEVEL: {_vampire.DetermineHungerLevel()}");
            Console.WriteLine($"FOLLOWERS: {_vampire.Followers.Count}");
            if (_vampire.Followers.Count(i => i.Follower.GetType() == (typeof(Vampire))) > 0)
                Console.WriteLine($"- VAMPIRES: {_vampire.Followers.Count(i => i.Follower.GetType() == (typeof(Vampire)))}");
            if (_vampire.Followers.Count(i => i.Follower.GetType() == (typeof(Zombie))) > 0)
                Console.WriteLine($"- ZOMBIES: {_vampire.Followers.Count(i => i.Follower.GetType() == (typeof(Zombie)))}");
            if (_vampire.Followers.Count(i => i.Follower.GetType() == (typeof(Cultist))) > 0)
                Console.WriteLine($"- CULTISTS: {_vampire.Followers.Count(i => i.Follower.GetType() == (typeof(Cultist)))}");
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
