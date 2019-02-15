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
        VampireLord _you;
        /// <summary>
        /// List of villages discovered by the Count
        /// </summary>
        List<Village> _villages;
        World _world;

        Random _random;

        private const int WORLD_SIZE = 3;
        public static bool IS_DEV = false;

        /// <summary>
        /// Hunger logic
        /// </summary>
        private const int HUNGER_WARNING_THRESHOLD = 5;
        private const int HUNGER_STARVING_THRESHOLD = 10;

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
            _villages = new List<Village>();

            // Add first village;
            _villages.Add(new Village()
            {
                Villagers = Randomizer.Roll(40, 2, _random),
                Suspicion = 0
            });

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
                Console.WriteLine($"LIVING VILLAGERS: {_villages[0].Villagers}");
                Console.WriteLine("");

                Console.WriteLine("");
                Console.WriteLine("Press ENTER to continue");
                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {} // No actions during the day yet. Could be permanent?

                // Villagers try to find vampire
                var location = new Location(Randomizer.Roll(1, WORLD_SIZE, _random), Randomizer.Roll(1, WORLD_SIZE, _random));
                while (_villages[0].LocationsSearched.Any(i => i.X == location.X && i.Y == location.Y))
                {
                    location = new Location(Randomizer.Roll(1, WORLD_SIZE, _random), Randomizer.Roll(1, WORLD_SIZE, _random));
                }

                if (IS_DEV)
                {
                    Console.WriteLine($"(DEV) HIDING AT: {_you.Location.X}:{_you.Location.Y}");
                    if (_villages[0].LocationFound)
                        Console.WriteLine($"(DEV) VILLAGERS FOUND:  {location.X}:{location.Y}");
                    else
                    {
                        Console.WriteLine($"(DEV) VILLAGERS PREVIOUSLY SEARCHED:  {ListUtil.ToStringFromLocations(_villages[0].LocationsSearched)}");
                        Console.WriteLine($"(DEV) VILLAGERS SEARCHED:  {location.X}:{location.Y}");
                    }
                }

                if (location.X == _you.Location.X && location.Y == _you.Location.Y || _villages[0].LocationFound)
                {
                    _villages[0].LocationFound = true;
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
                    _villages[0].LocationsSearched.Add(location);
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
                    infoText += "\nYou rise on your first night as a vampire. You feel a hunger you haven't felt before.\nYou feel compelled to feed on human blood.";
                if (hungerLevel >= HUNGER_WARNING_THRESHOLD)
                    infoText += $"\nWarning! You NEED to feed once every {HUNGER_STARVING_THRESHOLD} days!";
                if (hungerLevel >= HUNGER_STARVING_THRESHOLD)
                {
                    infoText += "\nYou are STARVING. You start taking damage, because you are not feeding!";
                    _you.Hitpoints--;
                    if (DeathCheck())
                        return false;
                }

                Console.WriteLine(infoText);
                Console.WriteLine("");

                // Stat Report
                Console.WriteLine($"HEALTH: {_you.Hitpoints}");
                Console.WriteLine($"HUNGER LEVEL: {hungerLevel}");
                Console.WriteLine($"LIVING VILLAGERS: {_villages[0].Villagers}");
                if (IS_DEV)
                    Console.WriteLine($"(DEV) VILLAGE SUSPICION: {_villages[0].Suspicion}");
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
                        _villages[0].LocationFound = false;
                        _villages[0].LocationsSearched.Clear();
                        _villages[0].Suspicion = Math.Min(1, _villages[0].Suspicion + ((float)Randomizer.Roll(3, 10, _random) / 100f)); // Can't get more suspicious than 1
                        break;
                    case "2":
                        // try to feed
                        if (_you.Feed(_villages[0]))
                        {
                            Console.WriteLine("You feed a villager successfully. You hunger recedes. ...For now\nThe village grows more suspicious.");
                            
                            // Effects on you
                            _you.LastFed = _day;
                            _you.Followers++;

                            // Effects on village
                            _villages[0].Villagers--;
                        }
                        else
                        {
                            Console.WriteLine("You fail your attempt to feed on a villager. The village grows more suspicious.");
                        }
                        _villages[0].Suspicion = Math.Min(1, _villages[0].Suspicion + ((float)Randomizer.Roll(3, 10, _random) / 100f)); // Can't get more suspicious than 1
                        break;
                    case "1":
                        Console.WriteLine("You hide for the night. The village grows less suspicious.");
                        _villages[0].Suspicion = Math.Max(0, _villages[0].Suspicion - ((float)Randomizer.Roll(20, 5, _random) / 100f)); // Can't get less suspicious than 0 
                        break;
                    default:
                        return true;
                       
                }

                // TODO: 
                // Decay villages' suspicion here. EXCEPT the one that was visited (if any)

                // Basic Win Condition - Will change
                if (_villages[0].Villagers <= 0)
                    _isGameOver = true;

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
