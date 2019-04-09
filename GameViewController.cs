using System;
using System.Collections.Generic;
using System.Linq;
using Count.Controllers;
using Count.Enums;
using Count.Models;
using Count.Models.Followers;

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

            _game = new Game();
            _game.World = _world = new WorldController();

            // Get starting World location
            _game.StartingWorldLocation = _game.World.GetUnusedWorldLocation();
            // Create castle
            _game.Castle = _castle = _world.AddLocationObject(new CastleController(_game.StartingWorldLocation)) as CastleController;
            // Create vampire lord
            _game.VampireLord = _vampire = new VampireLordController(_game);
            // Learn all basic research
            _game.KnownResearch.AddRange(_castle.ResearchOptions[0]);
        }

        private void Loop()
        {
            while (true)
            {
                // Phases
                while (Day()) ;
                while (Night()) ;


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
                Console.WriteLine("Press ENTER to continue");
                Console.ReadLine();

                Console.Clear();
                // Stat Report
                PrintStats();
                Console.WriteLine("");
                // TRY CREATE HERO for each village
                bool somethinghappened = false;
                foreach (var village in _world.LocationObjects.Where(i => i.GetType() == typeof(VillageController)))
                {
                    var hero = HeroController.TryCreateHero(typeof(Fighter), _game, village.WorldLocation);
                    if (hero != null)
                    {
                        Console.WriteLine($"{hero.Name} rises at {village.Name}");
                        _game.Heroes.Add(hero);
                        somethinghappened = true;
                    }
                }

                if (!somethinghappened)
                {
                    Console.WriteLine("Nothing happened today.");
                }

                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                { } // No actions during the day

                _vampire.Sleep();
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

                // Basic Win Condition - Will change **DEPRECATED
                if (_vampire.Blood >= BLOOD_WIN_COUNT)
                    _isGameOver = true;
            }
            return false;
        }

        private void EnterCastle()
        {
            var finishedEnterCastle = false;
            while (!finishedEnterCastle && _vampire.ActionPoints > 0)
            {
                Console.Clear();
                // Stat Report
                PrintStats();
                Console.WriteLine("");
                // Current Location
                // ----------------------------------------------------
                Console.WriteLine($"Current Location: {_castle.Name}");
                if (!string.IsNullOrWhiteSpace(_castle.Description))
                    Console.WriteLine($"{_castle.Description}");
                // ----------------------------------------------------
                Console.WriteLine("");
                Console.WriteLine("Actions: ");
                Console.WriteLine("");

                // Actions
                Console.WriteLine($"1. Leave Castle");
                Console.WriteLine($"2. Hatch Schemes");
                Console.WriteLine($"3. Fortify Castle");

                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {
                    case "1":
                        EnterWorld();
                        break;
                    case "2":
                        Schemes();
                        break;
                    default:
                        Console.WriteLine("Not available yet.");
                        Console.WriteLine("");
                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
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

        private void Schemes()
        {
            var finishedEnterCastle = false;
            while (!finishedEnterCastle && _vampire.ActionPoints > 0)
            {
                Console.Clear();
                // Stat Report
                PrintStats();
                Console.WriteLine("");
                // Current Location
                // ----------------------------------------------------
                Console.WriteLine($"Current Location: {_castle.Name}");
                if (!string.IsNullOrWhiteSpace(_castle.Description))
                    Console.WriteLine($"{_castle.Description}");
                // ----------------------------------------------------


                /*var hasNextResearchItem = _castle.ResearchOptions.Where(i => i.Key > _castle.ResearchPoints).OrderBy(j => j.Key).Any();
                var nextResearchItemName = hasNextResearchItem ? string.Join(",", _castle.ResearchOptions.Where(i => i.Key > _castle.ResearchPoints).OrderBy(j => j.Key).FirstOrDefault().Value.ToList().Select(i => i.Name)) : null;
                var nextResearchItemLevel = hasNextResearchItem ? (int?)(_castle.ResearchOptions.Where(i => i.Key > _castle.ResearchPoints).OrderBy(j => j.Key).FirstOrDefault().Key - _castle.ResearchPoints) : null;
                if (hasNextResearchItem)
                {
                    Console.WriteLine("----------------------------------------------------------------------------");
                    Console.WriteLine($"Next Spell: {nextResearchItemName}");
                    Console.WriteLine($"Blood Required: {nextResearchItemLevel}");
                }

                Console.WriteLine("----------------------------------------------------------------------------");*/
                Console.WriteLine("");
                Console.WriteLine("Actions: ");
                Console.WriteLine("");
                Console.WriteLine("1. Hatch new scheme");
                /*if (hasNextResearchItem)
                {
                    Console.WriteLine($"2. Read Tome of Blood Magic");
                }*/
                Console.WriteLine("Q. Go back to previous menu");
                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {
                    /*case "R":
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
                        break;*/
                    case "1":
                        NewScheme();
                        break;
                    case "Q":
                    case "q":
                        finishedEnterCastle = true;
                        break;

                }
            }
        }

        private void NewScheme()
        {
            var schemeCreatures = new List<FollowerController>();
            var finishedEnterCastle = false;
            while (!finishedEnterCastle && _vampire.ActionPoints > 0)
            {
                Console.Clear();
                // Stat Report
                PrintStats();
                Console.WriteLine("");
                // Current Location
                // ----------------------------------------------------
                Console.WriteLine($"Current Location: {_castle.Name}");
                if (!string.IsNullOrWhiteSpace(_castle.Description))
                    Console.WriteLine($"{_castle.Description}");
                // ----------------------------------------------------

                Console.WriteLine("");
                Console.WriteLine("Scheme: ");
                Console.WriteLine("Creatures: ");
                if (!schemeCreatures.Any())
                    Console.WriteLine($"No creatures yet");
                foreach (var creature in schemeCreatures)
                {
                    Console.WriteLine($"{creature.Name}");
                }

                Console.WriteLine("");
                Console.WriteLine("Actions: ");
                Console.WriteLine("");
                Console.WriteLine("1. Add creature to scheme");
                Console.WriteLine("Q. Go back to previous menu");
                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {
                    case "1":
                        AddCreatureToScheme(schemeCreatures);
                        break;
                    case "Q":
                    case "q":
                        finishedEnterCastle = true;
                        break;

                }
            }
        }
        private void AddCreatureToScheme(List<FollowerController> creatureList)
        {
            Console.Clear();
            // Stat Report
            Console.WriteLine("Available Creatures: ");
            int intOption = 1;
            foreach (var research in _game.KnownResearch)
            {
                Console.WriteLine($"{intOption++}. {research.Name}");
            }
            
            Console.WriteLine("");
            Console.Write(": ");

            var option = Console.ReadLine();
            Console.Clear();
            if (int.TryParse(option, out intOption))
            {
                int intIndex = intOption - 1;
                if (intIndex >= 0 && intIndex < _game.KnownResearch.Count)
                {
                    var research = _game.KnownResearch[intIndex];
                    creatureList.Add(FollowerController.TryCreateFollower(research.Unlocks, _castle.WorldLocation, true));
                }
            }
        }

        private void EnterWorld()
        {
            var finishedEnterWorld = false;
            while (!finishedEnterWorld && _vampire.ActionPoints > 0)
            {
                // Stat Report
                PrintStats();
                Console.WriteLine("");
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

                Console.WriteLine("");
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
                // Stat Report
                PrintStats();
                Console.WriteLine("");
                // Current Location
                // ----------------------------------------------------
                Console.WriteLine($"Current Location: {village.Name}");
                if (!string.IsNullOrWhiteSpace(village.Description))
                    Console.WriteLine($"{village.Description}");
                // ----------------------------------------------------
                Console.WriteLine("");
                Console.WriteLine("Actions: ");
                Console.WriteLine("");

                Console.WriteLine("1. Feed! (Get more blood)");
                Console.WriteLine("Q. Go back to previous menu");
                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {
                    case "1":
                        // try to feed
                        var feedStatus = _vampire.Feed();
                        switch (feedStatus)
                        {
                            case FeedStatus.INVALID:
                                Console.WriteLine("You dont have enough action points to feed.");
                                break;
                            case FeedStatus.FED:
                                Console.WriteLine("You feed successfully.");
                                finishedEnterVillage = true;
                                break;
                            case FeedStatus.BLOCKED:
                                Console.WriteLine("Can't feed when hero is present in village.");
                                break;
                            case FeedStatus.FAILED:
                                Console.WriteLine("You fail to feed.");
                                finishedEnterVillage = true;
                                break;
                        }
                        Console.WriteLine("");
                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        Console.Clear();
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
            PrintStats();
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

        private void PrintStats()
        {
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine($"BLOOD: {_vampire.Blood}/{BLOOD_WIN_COUNT}");
            Console.WriteLine($"INVESTIGATION: 0/10");
            Console.WriteLine("----------------------------------------------------------------------------");
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
