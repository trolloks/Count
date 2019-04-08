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
    /// NOTES: 
    /// Villages should NOT suspect you are a vampire in the beginning
    /// 
    /// Actions for vampire lord :
    /// devise plot
    /// research
    /// fortify-> protecting you during the day
    /// summon creature
    /// </summary>
    public class GameViewController
    {
        bool _isGameOver = false;

        internal Game _game;
        private VampireLordController _vampire;
        private WorldController _world;
        private CastleController _castle;

        public static bool IS_DEV = false;
        public static int BLOOD_WIN_COUNT = 100;

        public bool IS_FIRST = true;
        public int TUTORIAL_LEVEL = 1;

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
            /*Console.WriteLine("You are a vampire lord.");
            Console.WriteLine($"- You must FEED on a villager to receive BLOOD.");
            Console.WriteLine($"- Villages can be found by exploring the map");
            Console.WriteLine($"- BLOOD is required for discovering new unholy buildings that spawn new followers");
            Console.WriteLine($"- You need BLOOD to survive. The vampire's curse will steal blood from you at the end of every night. If you run out you will start to take damage.");
            Console.WriteLine("");
            Console.WriteLine($"***You WIN if you harvested {BLOOD_WIN_COUNT} Blood***");*/
            Console.WriteLine("");
            Console.WriteLine("Press ENTER to continue");
            Console.ReadLine();

            _game = new Models.Game();
            _game.World = _world = new WorldController();

            // Get starting World location
            _game.StartingWorldLocation = _game.World.GetUnusedWorldLocation();
            // Create castle
            _game.Castle = _castle = _world.AddLocationObject(new CastleController(_game.StartingWorldLocation)) as CastleController;
            // Create vampire lord
            _game.VampireLord = _vampire = new VampireLordController(_game);
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
                foreach (var village in _world.LocationObjects.Where(i => i.GetType() == typeof(VillageController)))
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
                Console.Clear();
                Console.WriteLine($"~~~ Day {_world.Day} (Night) ~~~");
                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                Console.ReadLine();

                EnterCastle();

                // After activities
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

        private void EnterCastle()
        {
            var infoText = string.Empty;
            /*if (_vampire.Blood <= VampireLordController.BLOOD_WARNING_THRESHOLD && _vampire.Blood > 0)
                infoText += $"\nWarning! You NEED find more blood soon!\nTry leaving the castle, to find a villager to feed on.";
            if (_vampire.Blood <= 0)
            {
                infoText += "\nYou are STARVING. You start taking damage, because you don't have blood!\nLeave the castle, and find a villager to feed on as soon as possible.";
                _vampire.Damage(1);
                if (_vampire.IsDead)
                    return;
            }*/

            if (IS_FIRST)
            {
                Console.Clear();
                PrintLocationObjectHeader(_castle, new string[] { });
                Console.WriteLine("You awaken.");
                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                Console.ReadLine();

                Console.Clear();
                PrintLocationObjectHeader(_castle, new string[] { });
                Console.WriteLine("You awaken. You collected some blood last night.");
                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                Console.ReadLine();

                Console.Clear();
                PrintStats(TUTORIAL_LEVEL);
                PrintLocationObjectHeader(_castle, new string[] { });
                Console.WriteLine("You awaken. You collected some blood last night.");
                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                Console.ReadLine();

                Console.Clear();
                PrintStats(TUTORIAL_LEVEL);
                PrintLocationObjectHeader(_castle, new string[] { });
                Console.WriteLine("Tonight, the ancient Tome of Blood Magic might yield some new undiscovered knowledge.");
                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                Console.ReadLine();
            }

            var finishedEnterCastle = false;
            while (!finishedEnterCastle && _vampire.ActionPoints > 0)
            {
                Console.Clear();
                PrintStats(TUTORIAL_LEVEL);
                PrintLocationObjectHeader(_castle, new string[] {});
                Console.WriteLine("Actions: ");
                Console.WriteLine("");
                
                // Actions
                if (_game.KnownResearch.Count > 0)
                {
                    // Can't leave castle before you researched something
                    Console.WriteLine($"L. Leave Castle");
                }
                
                Console.WriteLine($"R. Enter the library");
                //Console.WriteLine("I. Show your current information");

                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {
                    case "l":
                    case "L":
                        if (_game.KnownResearch.Count > 0)
                        {
                            // Can't leave castle before you researched something
                            EnterWorld();
                        }
                        break;
                    case "r":
                    case "R":
                        if (IS_FIRST)
                        {
                            Console.Clear();
                            PrintStats(TUTORIAL_LEVEL);
                            PrintLocationObjectHeader(_castle, new string[] { });
                            Console.WriteLine("You enter the library.");
                            Console.WriteLine("");
                            Console.WriteLine("Press ENTER to continue");
                            Console.ReadLine();
                        }

                        Research();
                        break;
                    /*case "i":
                    case "I":
                        PrintStats();
                        Console.WriteLine("");
                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;*/
                }
            }
        }

        private void Research()
        {
            var finishedEnterCastle = false;
            while (!finishedEnterCastle && _vampire.ActionPoints > 0)
            {
                Console.Clear();
                PrintStats(TUTORIAL_LEVEL);
                Console.WriteLine($"Current Location: {_castle.Name}'s Library");

                var hasNextResearchItem = _castle.ResearchOptions.Where(i => i.Key > _castle.ResearchPoints).OrderBy(j => j.Key).Any();
                var nextResearchItemName = hasNextResearchItem ? string.Join(",", _castle.ResearchOptions.Where(i => i.Key > _castle.ResearchPoints).OrderBy(j => j.Key).FirstOrDefault().Value.ToList().Select(i => i.Name)) : null;
                var nextResearchItemLevel = hasNextResearchItem ? (int?)(_castle.ResearchOptions.Where(i => i.Key > _castle.ResearchPoints).OrderBy(j => j.Key).FirstOrDefault().Key - _castle.ResearchPoints) : null;
                if (!IS_FIRST)
                {
                    if (hasNextResearchItem)
                    {
                        Console.WriteLine("----------------------------------------------------------------------------");
                        Console.WriteLine($"Next Spell: {nextResearchItemName}");
                        Console.WriteLine($"Blood Required: {nextResearchItemLevel}");
                    }
                }
                    
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("Actions: ");
                Console.WriteLine("");
                if (hasNextResearchItem)
                {
                    Console.WriteLine($"R. Read Tome of Blood Magic");
                }
                //Console.WriteLine("Q. Go back to previous menu");
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

                            var bloodToSpend = nextResearchItemLevel.Value;
                            var researchItems = _castle.Research(_vampire.Blood, nextResearchItemLevel.Value);
                            _vampire.SpendBlood(bloodToSpend);

                            if (IS_FIRST)
                            {
                                Console.Clear();
                                Console.WriteLine("The pages are empty...");
                                Console.WriteLine("");
                                Console.WriteLine("Press ENTER to continue");
                                Console.ReadLine();

                                Console.Clear();
                                Console.WriteLine("As you notice ");
                                Console.WriteLine("");
                                Console.WriteLine("Press ENTER to continue");
                                Console.ReadLine();

                                Console.Clear();
                                Console.WriteLine("You find that the book is filled with spells.\nSomehow you can make out the words on the first page.");
                                Console.WriteLine("");
                                Console.WriteLine("Press ENTER to continue");
                                Console.ReadLine();
                            }

                            Console.Clear();
                            Console.WriteLine("You uncover a new spell : ");
                            foreach (var researchItem in researchItems)
                            {
                                _game.KnownResearch.Add(researchItem);
                                Console.WriteLine($"- {researchItem.Name}!");
                            }

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

        private void EnterWorld()
        {
            var finishedEnterWorld = false;
            while (!finishedEnterWorld && _vampire.ActionPoints > 0)
            {
                Console.Clear();
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine($"~~~ Day {_world.Day} (Night) ~~~");
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("Current Location: (X)");
                StructureController currentLocationObject = _world.GetLocationObjectAtLocation(_vampire.WorldLocation);
                Console.WriteLine(currentLocationObject.Name);
                Console.WriteLine("");
                Console.WriteLine("Map:");

                var pointsOfInterest = new List<StructureController>();

                // Draw Map
                DrawMap(pointsOfInterest);

                Console.WriteLine("");
                Console.WriteLine("Points of interest:");
                int index = 0;
                foreach (var pointOfInterest in pointsOfInterest)
                {
                    Console.Write($"{++index}. {pointOfInterest.Name}");
                    Console.WriteLine("");
                }

                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine($"***{_vampire.ActionPoints} ACTION{(_vampire.ActionPoints > 1 ? "S" : "")} AVAILABLE****");
                Console.WriteLine("----------------------------------------------------------------------------");

                Console.WriteLine("Actions: ");
                Console.WriteLine("");

                // Actions
                Console.WriteLine($"{1}-{pointsOfInterest.Count}. Go to point of interest");
                Console.WriteLine($"E. Fly to location");

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
                            _vampire.MoveLocation(pointsOfInterest[poiOption - 1].WorldLocation);
                    }
                }
                else if (option == "E" || option == "e")
                {
                    // Enter location
                    if (currentLocationObject.GetType() == typeof(VillageController))
                    {
                        var village = currentLocationObject as VillageController;
                        EnterVillage(village);
                    }
                    else if (currentLocationObject.GetType() == typeof(CastleController))
                        finishedEnterWorld = true;
                    else
                        EnterLocationObject(currentLocationObject);
                }
                else if (option == "Q" || option == "q")
                {
                    finishedEnterWorld = true;
                }
            }
        }

        private void DrawMap(List<StructureController> pointsOfInterest)
        {
            int minY = int.MaxValue;
            int maxY = int.MinValue;
            int poi = 1;
            for (int i = 0; i < _world.LocationMap.GetLength(0); i++)
            {
                bool rowDrawn = false;
                for (int j = 0; j < _world.LocationMap.GetLength(1); j++)
                {
                    var currentLocation = new Location(i, j);
                    if (_vampire.WorldLocation.X == i && _vampire.WorldLocation.Y == j)
                    {
                        Console.Write("(X)");
                        rowDrawn = true;
                        continue;
                    }
                    else if (_world.Locations.Any(p => p.X == i && p.Y == j))
                    {
                        var locationObject = _world.GetLocationObjectAtLocation(currentLocation);
                        if (locationObject == null)
                        {
                            Console.Write("-");
                        }
                        else
                        {
                            pointsOfInterest.Add(locationObject);
                            Console.Write($"({poi++})");
                        }
                        minY = Math.Min(minY, j);
                        maxY = Math.Max(maxY, j);
                        rowDrawn = true;
                    }
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
                PrintLocationObjectHeader(village, new string[] { $"Population: {village.Size}" });
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

        /// <summary>
        /// Fallback area
        /// </summary>
        /// <param name="locationObject">Unknown Location Object</param>
        private void EnterLocationObject(StructureController locationObject)
        {
            var finishedEnterVillage = false;
            while (!finishedEnterVillage && _vampire.ActionPoints > 0)
            {
                Console.Clear();
                PrintLocationObjectHeader(locationObject, new string[0]);
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

        #endregion

        #region "ViewHelpers"

        private void PrintLocationObjectHeader(StructureController locationObject, string[] additionInfo, bool isNight = true)
        {
            // Stat Report
            //PrintStats();
            if (locationObject != null)
            {
                Console.WriteLine($"Current Location: {locationObject.Name}");
                if (!string.IsNullOrWhiteSpace(locationObject.Description))
                    Console.WriteLine($"{locationObject.Description}");
                foreach (var info in additionInfo)
                {
                    Console.WriteLine(info);
                }
            }
            Console.WriteLine("----------------------------------------------------------------------------");
            /*Console.WriteLine($"***{_vampire.ActionPoints} ACTION{(_vampire.ActionPoints > 1 ? "S" : "")} AVAILABLE****");
            Console.WriteLine("----------------------------------------------------------------------------");*/
        }

        private void PrintStats(int tutorialStep = int.MaxValue)
        {
            var totalZombies = _game.OwnedBuildings?.Where(i => i.GetType() == typeof(GraveyardController))?.Sum(j => (j as GraveyardController).Followers.Count);
            var totalVampires = _castle.Followers.Count;

            if (tutorialStep >= 2)
            {
                Console.WriteLine($"GOAL: {_vampire.Blood}/{BLOOD_WIN_COUNT} BLOOD");
            }

            if (tutorialStep >= 2)
            {
                Console.WriteLine($"HEALTH: {_vampire.Hitpoints}");
            }

            if (tutorialStep >= 1)
            {
                Console.WriteLine($"BLOOD: {_vampire.Blood}");
            }

            if (tutorialStep >= 2)
            {
                Console.WriteLine($"CORPSES: {_vampire.Corpses} (Resource used to create zombies from graveyards)");
                Console.WriteLine($"FOLLOWERS: {totalZombies + totalVampires}");
                if (_castle.Followers.Count > 0)
                    Console.WriteLine($"- VAMPIRES: {totalVampires}");
                if (totalZombies > 0)
                    Console.WriteLine($"- ZOMBIES: {totalZombies}");
            }
            
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

        #endregion
    }
}
