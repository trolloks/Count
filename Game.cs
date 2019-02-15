using Count.Models;
using Count.Utils;
using System;
using System.Linq;

namespace Count
{
    public class Game
    {
        long _day = 0;
        bool _isNight = true;
        bool _isGameOver = false;

        VampireLord _you;
        Village _village;
        World _world;

        Random _random;

        private const int WORLD_SIZE = 3;
        private const int BASE_FEED_DC = 10;
        private const int BASE_CHECK_ROLL = 20;
        private const bool IS_DEV = false;

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

            _random = new Random();

            _you = new VampireLord();
            _you.Hitpoints = 10;
            _you.LastFed = 1;
            _you.Location = new Location(Randomizer.Roll(1, WORLD_SIZE, _random), Randomizer.Roll(1, WORLD_SIZE, _random));

            _village = new Village();
            _village.Villagers = Randomizer.Roll(40, 2, _random);
            _village.Suspicion = 0;

            _world = new World();
            _world.Size = WORLD_SIZE; //WORLD_SIZExWORLD_SIZE
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

                if (DeathCheck())
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
            if (!DeathCheck())
            {
                _isNight = false;

                Console.Clear();
                Console.WriteLine($"~~~ Day {_day} (Day) ~~~");
                Console.WriteLine("");

                var infoText = "The sun rises again and you have to rest. Humanity uses this time to hunt you down";
                Console.WriteLine(infoText);
                Console.WriteLine("");

                // Stat Report
                Console.WriteLine($"HEALTH: {_you.Hitpoints}");
                Console.WriteLine($"FOLLOWERS: {_you.Followers}");
                Console.WriteLine($"LIVING VILLAGERS: {_village.Villagers}");
                Console.WriteLine("");

                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {
                   
                }

                // Villagers try to find vampire
                var location = new Location(Randomizer.Roll(1, WORLD_SIZE, _random), Randomizer.Roll(1, WORLD_SIZE, _random));
                while (_village.LocationsSearched.Any(i => i.X == location.X && i.Y == location.Y))
                {
                    location = new Location(Randomizer.Roll(1, WORLD_SIZE, _random), Randomizer.Roll(1, WORLD_SIZE, _random));
                }

                if (IS_DEV)
                {
                    Console.WriteLine($"(DEV) HIDING AT: {_you.Location.X}:{_you.Location.Y}");
                    if (_village.LocationFound)
                        Console.WriteLine($"(DEV) VILLAGERS FOUND:  {location.X}:{location.Y}");
                    else
                    {
                        Console.WriteLine($"(DEV) VILLAGERS PREVIOUSLY SEARCHED:  {ListUtil.ToStringFromLocations(_village.LocationsSearched)}");
                        Console.WriteLine($"(DEV) VILLAGERS SEARCHED:  {location.X}:{location.Y}");
                    }
                }

                if (location.X == _you.Location.X && location.Y == _you.Location.Y || _village.LocationFound)
                {
                    _village.LocationFound = true;
                    Console.WriteLine("The villagers have found your hiding place!");
                    if (_you.Followers > 0)
                    {
                        Console.WriteLine("A follower gives his life to save yours. Move your hiding place!");
                        _you.Followers--;
                    }
                    else
                    {
                        Console.WriteLine("With no-one to protect you. You get killed by the villagers!");
                        _you.Hitpoints = 0;
                    }
                }
                else
                {
                    Console.WriteLine("The villagers failed to find your hiding place.");
                    // Search better each round
                    _village.LocationsSearched.Add(location);
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
            if (!DeathCheck())
            {
                _isNight = true;

                // Setup
                var hungerLevel = _day - _you.LastFed;

                Console.Clear();
                Console.WriteLine($"~~~ Day {_day} (Night) ~~~");
                Console.WriteLine("");

                var infoText = "Night falls. You can yet again roam the land.";
                if (_day == 1)
                {
                    infoText += "\nYou rise on your first night as a vampire. You feel a hunger you haven't felt before.\nYou feel compelled to feed on human blood.";
                }

                if (hungerLevel >= 5)
                    infoText += "\nYou are STARVING. You NEED to feed!";
                if (hungerLevel >= 10)
                {
                    infoText += "\nYou start taking damage, because you are not feeding!";
                    _you.Hitpoints--;
                    if (DeathCheck())
                        return false;
                }

                Console.WriteLine(infoText);
                Console.WriteLine("");

                // Stat Report
                Console.WriteLine($"HEALTH: {_you.Hitpoints}");
                Console.WriteLine($"HUNGER LEVEL: {hungerLevel}");
                Console.WriteLine($"LIVING VILLAGERS: {_village.Villagers}");
                if (IS_DEV)
                    Console.WriteLine($"(DEV) VILLAGE SUSPICION: {_village.Suspicion}");
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
                        _you.Location = new Location(Randomizer.Roll(1, WORLD_SIZE, _random), Randomizer.Roll(1, WORLD_SIZE, _random));
                        _village.LocationFound = false;
                        _village.LocationsSearched.Clear();
                        _village.Suspicion = Math.Min(1, _village.Suspicion + ((float)Randomizer.Roll(3, 10, _random) / 100f)); // Can't get more suspicious than 1
                        break;
                    case "2":
                        var feedRoll = Randomizer.Roll(1, BASE_CHECK_ROLL, _random);
                        var feedCheck = feedRoll >= (BASE_FEED_DC + Math.Round(((float)BASE_FEED_DC) * _village.Suspicion));
                        if (IS_DEV)
                        {
                            Console.WriteLine($"(DEV) FEED CHECK: {feedRoll}");
                            Console.WriteLine($"(DEV) FEED DC CHECK: {(BASE_FEED_DC + Math.Round(((float)BASE_FEED_DC) * _village.Suspicion))}");
                        }
                        
                        // try to feed
                        if (feedCheck)
                        {
                            Console.WriteLine("You feed a villager successfully. You hunger recedes. ...For now\nThe village grows more suspicious.");
                            
                            // Effects on you
                            _you.LastFed = _day;
                            _you.Followers++;

                            // Effects on village
                            _village.Villagers--;
                        }
                        else
                        {
                            Console.WriteLine("You fail your attempt to feed on a villager. The village grows more suspicious.");
                        }
                        _village.Suspicion = Math.Min(1, _village.Suspicion + ((float)Randomizer.Roll(3, 10, _random) / 100f)); // Can't get more suspicious than 1
                        break;
                    case "1":
                        Console.WriteLine("You hide for the night. The village grows less suspicious.");
                        _village.Suspicion = Math.Max(0, _village.Suspicion - ((float)Randomizer.Roll(20, 5, _random) / 100f)); // Can't get less suspicious than 0 
                        break;
                    default:
                        return true;
                       
                }

                // You win!
                if (_village.Villagers <= 0)
                {
                    _isGameOver = true;
                }

                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                Console.ReadLine();
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Checks if you are dead
        /// </summary>
        /// <returns>Returns true if you are dead</returns>
        private bool DeathCheck()
        {
            return _you.Hitpoints <= 0; 
        }

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
