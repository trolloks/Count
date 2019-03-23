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
    /// 1. Make map much smaller for now.
    /// 2. Remove suspicion -> work with population size
    /// 3. More direct control over minions? 
    /// 
    /// TODO: new plan
    /// instead of buildings. new resource corpses for zombies. mega corpses for heros. to make special creatures. fighter -> death knight?
    /// neutral factions?
    /// graveyards should not be built should be seeded and found
    /// 
    /// TODO: should use status for village empty
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
        public static int BLOOD_WIN_COUNT = 100;

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
            Console.WriteLine($"- You must FEED on a villager to receive BLOOD.");
            Console.WriteLine($"- Villages can be found by exploring the map");
            Console.WriteLine($"- BLOOD is required for discovering new unholy buildings that spawn new followers");
            Console.WriteLine($"- You need BLOOD to survive. The vampire's curse will steal blood from you at the end of every night. If you run out you will start to take damage.");
            Console.WriteLine("");
            Console.WriteLine($"***You WIN if you harvested {BLOOD_WIN_COUNT} Blood***");
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
                // Pay the blood curse
                _vampire.PayBlood(); 
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
                if (_vampire.Blood <= VampireLordController.BLOOD_WARNING_THRESHOLD && _vampire.Blood > 0)
                    infoText += $"\nWarning! You NEED find more blood soon!\nTry leaving the castle, to find a villager to feed on.";
                if (_vampire.Blood <= 0)
                {
                    infoText += "\nYou are STARVING. You start taking damage, because you don't have blood!\nLeave the castle, and find a villager to feed on as soon as possible.";
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
                    if (!string.IsNullOrWhiteSpace(infoText))
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
                    Console.WriteLine("2. Research new abilities");
                    Console.WriteLine("3. Stay the night");

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
                        case "3":
                            _vampire.TryExert(1);
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

                Console.WriteLine("The vampire's curse claims it's price in blood. (You lose 1 blood)");
                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                Console.ReadLine();

                // Basic Win Condition - Will change **DEPRECATED
                if (_vampire.Blood >= BLOOD_WIN_COUNT)
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
                var hasNextResearchItem = _castle.ResearchOptions.Where(i => i.Key > _castle.ResearchPoints).OrderBy(j => j.Key).Any();
                var nextResearchItemName = hasNextResearchItem ? string.Join(",", _castle.ResearchOptions.Where(i => i.Key > _castle.ResearchPoints).OrderBy(j => j.Key).FirstOrDefault().Value.ToList().Select(i => i.Name)) : null;
                var nextResearchItemLevel = hasNextResearchItem ? (int?)(_castle.ResearchOptions.Where(i => i.Key > _castle.ResearchPoints).OrderBy(j => j.Key).FirstOrDefault().Key - _castle.ResearchPoints) : null;
                if (hasNextResearchItem)
                {
                    Console.WriteLine("----------------------------------------------------------------------------");
                    Console.WriteLine($"The library of {_castle.Name} offers: ");
                    Console.WriteLine($"Next Unlock: {nextResearchItemName}");
                    Console.WriteLine($"Blood Required: {nextResearchItemLevel}");
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
                if (hasNextResearchItem)
                {
                    Console.WriteLine($"R. Research ancient texts (1 Action, {nextResearchItemLevel} Blood)");
                    Console.WriteLine($"- Unearth new knowledge to teach you about unholy abilities."); 
                }
                Console.WriteLine("");
                Console.WriteLine("Q. Go back to previous menu");
                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {
                    case "R":
                    case "r":
                        if (hasNextResearchItem)
                        {
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

                            var bloodToSpend = nextResearchItemLevel.Value;
                            var researchItems = _castle.Research(_vampire.Blood, nextResearchItemLevel.Value);
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
                        }
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

                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();

                int poiOption = -1;
                if (int.TryParse(option, out poiOption))
                {
                    if ((poiOption - 1) < pointsOfInterest.Count && (poiOption - 1) >= 0)
                    {
                        var pointOfInterest = pointsOfInterest[poiOption - 1];
                        if (pointOfInterest != null)
                            _vampire.MoveLocation(pointsOfInterest[poiOption - 1].WorldLocation, pointsOfInterest[poiOption - 1].RegionLocation);
                    }
                }
                else if (option == "E" || option == "e")
                {
                    // Enter location
                    if (currentLocationObject.GetType() == typeof(VillageController))
                    {
                        var village = currentLocationObject as VillageController;
                        EnterVillage(village);
                        if (!_game.KnownVillages.Contains(village))
                            _game.KnownVillages.Add(village);
                    }
                    else if (currentLocationObject.GetType() == typeof(CastleController))
                        finishedEnterRegion = true;
                    else if (currentLocationObject.GetType() == typeof(UnexploredController))
                        EnterUnexploredArea(currentLocationObject as UnexploredController);
                    else
                        EnterLocationObject(currentLocationObject);
                }
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
                Console.WriteLine("");
                Console.WriteLine($"***{_vampire.ActionPoints} ACTION{(_vampire.ActionPoints > 1 ? "S" : "")} AVAILABLE****");
                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("Actions: ");
                Console.WriteLine("");

                if (village.Size > 0)
                {
                    Console.WriteLine("1. Feed! (1 Action)");
                    Console.WriteLine($"- Satisfy your hunger. If you succeed you will receive BLOOD. (You might create a vampire in the process)");
                    Console.WriteLine("");
                }

                Console.WriteLine("Q. Go back to previous menu");
                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {
                    case "1":
                        if (village.Size > 0)
                        {
                            // Exert after an action
                            if (_vampire.TryExert(1))
                            {
                                // try to feed
                                var feedStatus = _vampire.Feed();
                                switch (feedStatus)
                                {
                                    case FeedStatus.FED:
                                        Console.WriteLine("You feed a villager successfully. You hunger recedes. ...For now");
                                        break;
                                    case FeedStatus.CONVERTED:
                                        Console.WriteLine("You feed a villager successfully. You hunger recedes. ...BUT you have created another like yourself. For now he will serve you.");
                                        break;
                                    case FeedStatus.FAILED:
                                        Console.WriteLine("You fail your attempt to feed on a villager.");
                                        break;
                                }

                                finishedEnterVillage = true;
                            }
                            else
                            {
                                Console.WriteLine("You dont have enough action points to feed.");
                            }
                        }
                        else
                        {
                            // nothing
                        }
                        Console.WriteLine("");
                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;
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

            Console.WriteLine($"GOAL: {_vampire.Blood}/{BLOOD_WIN_COUNT} BLOOD");
            Console.WriteLine($"HEALTH: {_vampire.Hitpoints}");
            Console.WriteLine($"BLOOD: {_vampire.Blood} (Resource used to feed your hunger as well as buy buildings/research)");
            Console.WriteLine($"CORPSES: {_vampire.Corpses} (Resource used to create zombies from graveyards)");
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
            Console.WriteLine($"Congratulations! You have been successfully harvested {BLOOD_WIN_COUNT} Blood.");
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
