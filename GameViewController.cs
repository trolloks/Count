using System;
using System.Collections.Generic;
using System.Linq;
using Count.Controllers;
using Count.Enums;
using Count.Models;

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
    /// NEW PLAN: 
    /// Feed to get blood points. Spend blood points on buildings. that generate followers. villages revolt can destroy buildings
    /// Vampire followers should do anything you do automatically
    /// POI should not all be revealed
    /// 
    /// Zombies could be created either by building a graveyard or researching a raise zombie spell?
    /// 
    /// VILLAGERS = RESOURCES
    /// WINCON = PER LEVEL
    /// 
    /// NEW NEW PLAN :
    /// 
    /// Create MUCH bigger map! Much bigger amount of zombies to win. Can create multiple graveyards. maybe increase price. graveyards only on known locations
    /// 
    /// Vampires should also defend you. but just in castle
    /// 
    /// </summary>
    public class GameViewController
    {
        bool _isGameOver = false;

        internal Models.Game _game;
        private VampireLordController _vampire;
        private WorldController _world;
        private CastleController _castle;

        public static bool IS_DEV = false;
        public static int ZOMBIE_WIN_COUNT = 25;

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
            Console.WriteLine("You are a vampire lord.");
            Console.WriteLine($"- You must FEED on a villager every {VampireLordController.HUNGER_STARVING_THRESHOLD} days.");
            Console.WriteLine($"- Villages can be found by exploring the map");
            Console.WriteLine($"- FEEDING rewards you with BLOOD.");
            Console.WriteLine($"- BLOOD is required for discovering new unholy buildings that spawn new followers");
            Console.WriteLine("");
            Console.WriteLine($"***You WIN if you create {ZOMBIE_WIN_COUNT} Zombies***");
            Console.WriteLine("");
            Console.WriteLine("Press ENTER to continue");
            Console.ReadLine();

            _game = new Models.Game();
            _game.World = _world = new WorldController();

            // Get starting World location
            _game.StartingWorldLocation = _game.World.GetUnusedWorldLocation();
            // Generate starting Region
            var startingRegion = _game.World.AddRegionAtLocation(_game.StartingWorldLocation);
            // Get starting Region location
            _game.StartingRegionLocation = startingRegion.GetUnusedRegionLocation();
            // Create castle
            _game.Castle = _castle = startingRegion.AddLocationObject(new CastleController(_game.StartingWorldLocation, _game.StartingRegionLocation)) as CastleController;
            // Create vampire
            _game.VampireLord = _vampire = new VampireLordController(_game);
            _game.KnownLocations.Add(_game.StartingRegionLocation);
        }

        private void Loop()
        {
            while (true)
            {
                // Phases
                while (Night()) ;
                while (Day()) ;

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
                bool somethingHappened = false;
                foreach (var village in _game.KnownVillages)
                {
                    somethingHappened = village.Upkeep(_game) || somethingHappened;
                }
                if (!somethingHappened)
                {
                    Console.WriteLine("The day passes quietly. You have no threats yet...");
                }
                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                { } // No actions during the day yet. Could be permanent?

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

                /*Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                Console.ReadLine();*/
            }
            return false;
        }
        #endregion
        #region "Night"
        private bool Night()
        {
            if (!_vampire.IsDead)
            {
                var infoText = string.Empty;
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
                    Console.WriteLine("----------------------------------------------------------------------------");
                    Console.WriteLine($"Welcome to {_castle.Name}");
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
                    Console.WriteLine("1. Leave Castle");
                    Console.WriteLine("2. Unlock new unholy buildings");

                    Console.WriteLine("");
                    Console.Write(": ");

                    var option = Console.ReadLine();
                    Console.Clear();
                    switch (option)
                    {
                        case "1":
                            EnterRegion();
                            break;
                        case "2":
                            Research();
                            break;
                    }
                }

                Console.Clear();
                Console.WriteLine("During the the night the following happened:");
                bool somethingHappened = false;
                // Upkeep
                somethingHappened = _castle.Upkeep(_game) || somethingHappened;
                foreach (var locationObject in _game.OwnedBuildings)
                {
                    if (locationObject.GetType() == typeof(GraveyardController))
                    {
                        var graveyard = locationObject as GraveyardController;
                        somethingHappened = graveyard.Upkeep(_game) || somethingHappened;
                    }
                }

                if (!somethingHappened)
                {
                    Console.WriteLine("Nothing interesting.");
                }

                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                Console.ReadLine();

                // Basic Win Condition - Will change **DEPRECATED
                if (_game.OwnedBuildings?.Where(i => i.GetType() == typeof(GraveyardController))?.Sum(j => (j as GraveyardController). Followers.Count) >= ZOMBIE_WIN_COUNT)
                    _isGameOver = true;
            }
            return false;
        }

        private void Research()
        {
            var finishedEnterCastle = false;
            while (!finishedEnterCastle && _vampire.ActionPoints > 0)
            {
                Console.Clear();
                Console.WriteLine($"~~~ Day {_world.Day} (Night) ~~~");
                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine($"The library of {_castle.Name} offers: ");
                var hasNextResearchItem = _castle.ResearchOptions.Where(i => i.Key > _castle.ResearchPoints).OrderBy(j => j.Key).Any();
                var nextResearchItemName = hasNextResearchItem ? string.Join(",", _castle.ResearchOptions.Where(i => i.Key > _castle.ResearchPoints).OrderBy(j => j.Key).FirstOrDefault().Value.ToList().Select(i => i.Name)) : "N/A";
                var nextResearchItemLevel = hasNextResearchItem ? (_castle.ResearchOptions.Where(i => i.Key > _castle.ResearchPoints).OrderBy(j => j.Key).FirstOrDefault().Key - _castle.ResearchPoints) : int.MaxValue;
                Console.WriteLine($"Next Unlock: {nextResearchItemName}");
                Console.WriteLine($"Blood Required: {nextResearchItemLevel}");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("");
                PrintStats();
                Console.WriteLine("");
                Console.WriteLine($"***{_vampire.ActionPoints} ACTION{(_vampire.ActionPoints > 1 ? "S" : "")} AVAILABLE****");
                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("Actions: ");
                Console.WriteLine("");
                int researchIndex = 0;
                foreach (var researchItem in _game.KnownResearch)
                {
                    Console.WriteLine($"{++researchIndex}. Build {researchItem.Name} (1 Action, {researchItem.Blood} Blood)");
                }
                Console.WriteLine($"R. Research ancient texts (1 Action, {nextResearchItemLevel} Blood)");
                Console.WriteLine($"- Unearth new knowledge to teach you about unholy buildings.");
                Console.WriteLine("");
                /*Console.WriteLine("2. Convert Villager into follower (1 Action)");
                Console.WriteLine("\t- Followers help you further your schemes. They would even give their lives to protect you against the villagers.");*/
                Console.WriteLine("Q. Go back to previous menu");
                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();
                int buildOption = -1;
                if (int.TryParse(option, out buildOption))
                {
                    if (buildOption < _game.KnownResearch.Count + 1 && buildOption > 0)
                    {
                        var researchItem = _game.KnownResearch[buildOption - 1];
                        if (_vampire.Blood >= researchItem.Blood)
                        {
                            var newLocation = _world.GetRegion(_vampire.WorldLocation).GetUnusedRegionLocation(_game.KnownLocations);
                            if (newLocation != null)
                            {
                                if (_vampire.ActionPoints >= 1)
                                {
                                    Console.WriteLine($"- A {researchItem.Name} appears on the map");
                                    var currentRegion = _world.GetRegion(_vampire.WorldLocation);
                                    var newResearchedLocationObject = researchItem.Unlocks.GetConstructor(new Type[] { typeof(Location), typeof(Location) }).Invoke(new object[] { _vampire.WorldLocation, newLocation });
                                    _game.OwnedBuildings.Add(currentRegion.AddLocationObject(newResearchedLocationObject as StructureController));

                                    _vampire.TryExert(1);
                                    _vampire.SpendBlood(researchItem.Blood);
                                    finishedEnterCastle = true;

                                    Console.WriteLine("");
                                    Console.WriteLine("Press ENTER to continue");
                                    Console.ReadLine();
                                }
                                else
                                {
                                    Console.WriteLine("You dont have enough action points to research this.");
                                    Console.WriteLine("");
                                    Console.WriteLine("Press ENTER to continue");
                                    Console.ReadLine();
                                }

                            }
                            else
                            {
                                Console.WriteLine("There is no space for this item, discover more empty spaces to build this");
                                Console.WriteLine("");
                                Console.WriteLine("Press ENTER to continue");
                                Console.ReadLine();
                            }
                        }
                        else
                        {
                            Console.WriteLine("You dont have enough blood to build this, feed on villagers to increase your blood.");
                            Console.WriteLine("");
                            Console.WriteLine("Press ENTER to continue");
                            Console.ReadLine();
                        }
                    }
                }
                else
                    switch (option)
                    {
                        case "R":
                        case "r":
                            // research
                            if (_vampire.Blood < nextResearchItemLevel)
                            {
                                Console.WriteLine("You dont have enough blood to research this, feed on villagers to increase your blood.");
                                Console.WriteLine("");
                                Console.WriteLine("Press ENTER to continue");
                                Console.ReadLine();
                                break;
                            }

                            if (_vampire.ActionPoints < 1)
                            {
                                Console.WriteLine("You dont have enough action points to research this.");
                                Console.WriteLine("");
                                Console.WriteLine("Press ENTER to continue");
                                Console.ReadLine();
                                break;
                            }

                            var bloodToSpend = nextResearchItemLevel;
                            var researchItems = _castle.Research(_vampire.Blood, nextResearchItemLevel);
                            _vampire.SpendBlood(bloodToSpend);

                            Console.WriteLine("You spend the night reading through old tomes, trying to discern anything of value. You discover the following lost knowledge: ");
                            foreach (var researchItem in researchItems)
                            {
                                _game.KnownResearch.Add(researchItem);
                                Console.WriteLine($"- {researchItem.Name}!");
                            }

                            _vampire.TryExert(1);
                            finishedEnterCastle = true;

                            Console.WriteLine("");
                            Console.WriteLine("Press ENTER to continue");
                            Console.ReadLine();

                            break;
                        case "Q":
                        case "q":
                            finishedEnterCastle = true;
                            break;

                    }
            }
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
                Console.WriteLine("Current Location: (X)");
                var region = _world.GetRegion(_vampire.WorldLocation);
                StructureController currentLocationObject = null;
                // Only check if known
                if (_game.KnownLocations.Any(p => p.X == _vampire.RegionLocation.X && p.Y == _vampire.RegionLocation.Y))
                    currentLocationObject = region.GetLocationObjectAtLocation(_vampire.RegionLocation);
                else
                    currentLocationObject = new UnexploredController(_vampire.WorldLocation, _vampire.RegionLocation);

                if (currentLocationObject.GetType() == typeof(VillageController))
                {
                    var currentVillage = currentLocationObject as VillageController;
                    Console.WriteLine(currentVillage.Name + (currentVillage.Suspicion >= VillageController.SUSPICION_WARNING_THRESHOLD ? "(Alerted)" : ""));
                }
                else
                    Console.WriteLine(currentLocationObject.Name);

                Console.WriteLine("");
                Console.WriteLine("Map:");

                var pointsOfInterest = new List<StructureController>();

                // Draw Map
                DrawMap(region, pointsOfInterest);

                Console.WriteLine("");
                Console.WriteLine("Points of interest:");
                int index = 0;
                foreach (var pointOfInterest in pointsOfInterest)
                {
                    Console.Write($"{++index}. {pointOfInterest.Name}");
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
                //if (currentLocationObject.GetType() == typeof(VillageController))
                //Console.WriteLine($"C. Send Cultist to \"convince\" village (1 Action)");

                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();

                int poiOption = -1;
                if (int.TryParse(option, out poiOption))
                {
                    var pointOfInterest = pointsOfInterest[poiOption - 1];
                    if (pointOfInterest != null)
                        _vampire.MoveLocation(pointsOfInterest[poiOption - 1].WorldLocation, pointsOfInterest[poiOption - 1].RegionLocation);
                }
                else if (option == "E" || option == "e")
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
                        {
                            EnterVillage(village);
                            if (!_game.KnownVillages.Contains(village))
                                _game.KnownVillages.Add(village);
                        }
                    }
                    else if (currentLocationObject.GetType() == typeof(CastleController))
                        finishedEnterRegion = true;
                    else if (currentLocationObject.GetType() == typeof(UnexploredController))
                        EnterUnexploredArea(currentLocationObject as UnexploredController);
                    else
                        EnterLocationObject(currentLocationObject);
                }
                /*else if (option == "C")
                {
                    if (currentLocationObject.GetType() == typeof(VillageController))
                    {
                        var village = currentLocationObject as VillageController;
                        var hasCultist = _vampire.Followers.Any(i => i.Follower.GetType() == typeof(Cultist));
                        if (hasCultist)
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
                }*/
                else if (option == "Q" || option == "q")
                {
                    finishedEnterRegion = true;
                }
            }
        }

        private void DrawMap(RegionController region, List<StructureController> pointsOfInterest)
        {
            int minY = int.MaxValue;
            int maxY = int.MinValue;
            int poi = 1;
            for (int i = 0; i < region.LocationObjects.GetLength(0); i++)
            {
                bool rowDrawn = false;
                for (int j = 0; j < region.LocationObjects.GetLength(1); j++)
                {
                    var currentLocation = new Location(i, j);
                    if (_vampire.RegionLocation.X == i && _vampire.RegionLocation.Y == j)
                    {
                        Console.Write("(X)");
                        rowDrawn = true;
                        continue;
                    }
                    else if (_game.KnownLocations.Any(p => p.X == i && p.Y == j))
                    {
                        var locationObject = region.GetLocationObjectAtLocation(currentLocation);
                        if (locationObject == null)
                        {
                            Console.Write("-");
                        }
                        else
                        {
                            pointsOfInterest.Add(locationObject);
                            Console.Write($"({poi++}:L)");
                        }
                        minY = Math.Min(minY, j);
                        maxY = Math.Max(maxY, j);
                        rowDrawn = true;
                    }
                    else if (_game.KnownLocations.Any(p => ((p.X <= i + 1) && (p.X >= i - 1)) && ((p.Y <= j + 1) && (p.Y >= j - 1))))
                    {
                        pointsOfInterest.Add(new UnexploredController(_vampire.WorldLocation, currentLocation));
                        Console.Write($"({poi++})");
                        minY = Math.Min(minY, j);
                        maxY = Math.Max(maxY, j);
                        rowDrawn = true;
                    }
                    // WIP
                    /*else if (j > minY && j < maxY)
                    {
                        Console.Write("?");
                    }*/

                }

                if (rowDrawn)
                    Console.WriteLine("");
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
                Console.WriteLine($"- Satisfy your hunger. If you don't feed once every {VampireLordController.HUNGER_STARVING_THRESHOLD} days you will start taking damage.");
                Console.WriteLine("");
                /*Console.WriteLine("2. Convert Villager into follower (1 Action)");
                Console.WriteLine("\t- Followers help you further your schemes. They would even give their lives to protect you against the villagers.");*/
                Console.WriteLine("Q. Go back to previous menu");
                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {
                    case "1":
                        // Exert after an action
                        if (_vampire.TryExert(1))
                        {
                            // try to feed
                            var feedStatus = _vampire.Feed();
                            switch (feedStatus)
                            {
                                case FeedStatus.FED:
                                    Console.WriteLine("You feed a villager successfully. You hunger recedes. ...For now\nThe village grows more suspicious.");
                                    break;
                                case FeedStatus.CONVERTED:
                                    Console.WriteLine("You feed a villager successfully. You hunger recedes. ...BUT you have created another like yourself. For now he will serve you.\nThe village grows more suspicious.");
                                    break;
                                case FeedStatus.FAILED:
                                    Console.WriteLine("You fail your attempt to feed on a villager. The village grows more suspicious.");
                                    break;
                            }


                            village.IncreaseSuspicion();
                            /*foreach (var othervillage in _game.KnownVillages.Where(i => !i.Equals(village)))
                            {
                                othervillage.DecreaseSuspicion();
                            }*/
                            finishedEnterVillage = true;
                        }
                        else
                        {
                            Console.WriteLine("You dont have enough action points to feed.");
                        }

                        Console.WriteLine("");
                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;
                    /*case "2":
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
                        break;*/
                    case "Q":
                    case "q":
                        finishedEnterVillage = true;
                        break;

                }
            }
        }

        private void EnterLocationObject(StructureController locationObject)
        {
            var finishedEnterVillage = false;
            while (!finishedEnterVillage && _vampire.ActionPoints > 0)
            {
                Console.Clear();
                Console.WriteLine($"~~~ Day {_world.Day} (Night) ~~~");
                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine($"{locationObject.Name}");
                Console.WriteLine($"{locationObject.Description}");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("");
                PrintStats();
                Console.WriteLine("");
                Console.WriteLine($"***{_vampire.ActionPoints} ACTION{(_vampire.ActionPoints > 1 ? "S" : "")} AVAILABLE****");
                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("Actions: ");
                Console.WriteLine("");

                Console.WriteLine("Q. Go back to previous menu");
                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {
                    case "Q":
                    case "q":
                        finishedEnterVillage = true;
                        break;
                }
            }
        }

        private void EnterUnexploredArea(UnexploredController unexploredArea)
        {
            var finishedEnterUnexploredArea = false;
            while (!finishedEnterUnexploredArea && _vampire.ActionPoints > 0)
            {
                Console.Clear();
                Console.WriteLine($"~~~ Day {_world.Day} (Night) ~~~");
                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine($"{unexploredArea.Name}");
                Console.WriteLine($"{unexploredArea.Description}");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("");
                PrintStats();
                Console.WriteLine("");
                Console.WriteLine($"***{_vampire.ActionPoints} ACTION{(_vampire.ActionPoints > 1 ? "S" : "")} AVAILABLE****");
                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("Actions: ");
                Console.WriteLine("");

                Console.WriteLine("1. Explore the area (1 Action)");
                Console.WriteLine("Q. Go back to previous menu");
                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {
                    case "1":
                        if (_vampire.TryExert(1))
                        {
                            var locationObject = unexploredArea.Explore(_game);
                            if (locationObject != null)
                            {
                                Console.WriteLine($"You found a {locationObject.Name}");
                            }
                            else
                                Console.WriteLine("You found nothing.");

                            finishedEnterUnexploredArea = true;
                        }

                        Console.WriteLine("");
                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;
                    case "Q":
                    case "q":
                        finishedEnterUnexploredArea = true;
                        break;
                }
            }
        }
      
        #endregion

        private void PrintStats()
        {
            var totalZombies = _game.OwnedBuildings?.Where(i => i.GetType() == typeof(GraveyardController))?.Sum(j => (j as GraveyardController).Followers.Count);
            var totalVampires = _castle.Followers.Count;

            Console.WriteLine($"GOAL: {(_game.OwnedBuildings.Count > 0 ? (totalZombies) : 0)}/{ZOMBIE_WIN_COUNT} ZOMBIES");
            Console.WriteLine($"HEALTH: {_vampire.Hitpoints}");
            Console.WriteLine($"HUNGER LEVEL: {_vampire.DetermineHungerLevel()}");
            Console.WriteLine($"BLOOD: {_vampire.Blood}");
            Console.WriteLine($"EXPLORED VILLAGES: {_game.KnownVillages.Count}");
            Console.WriteLine($"FOLLOWERS: {totalZombies + totalVampires}");
            if (_castle.Followers.Count > 0)
                Console.WriteLine($"- VAMPIRES: {totalVampires}");
            if (totalZombies > 0)
                Console.WriteLine($"- ZOMBIES: {totalZombies}");
        }

        private void Win()
        {
            Console.Clear();
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("GAME OVER");
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine($"Congratulations! You have been successfully created {ZOMBIE_WIN_COUNT} Zombies.");
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
