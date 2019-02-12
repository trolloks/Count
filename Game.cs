using Count.Models;
using Count.Utils;
using System;

namespace Count
{
    public class Game
    {
        long _day = 0;
        bool _isNight = true;
        bool _isGameOver = false;

        VampireLord _you;
        Village _village;

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

            _you = new VampireLord();
            _you.Hitpoints = 10;
            _you.LastFed = 1;

            _village = new Village();
            _village.Villagers = Randomizer.Roll(5, 6);
            _village.Suspicion = 0;
        }

        private void Loop()
        {
            while (true)
            {
                // Start of new day
                _day++;

                // Phases
                Night();
                Day();

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

        private void Day()
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

                var option = Console.ReadLine();
                // TODO: Remove kill game hack
                if (option == "q")
                {
                    _isGameOver = true;
                }
            }
        }

        private void Night()
        {
            if (!DeathCheck())
            {
                _isNight = true;

                // Stats
                var hungerLevel = _day - _you.LastFed;

                Console.Clear();
                Console.WriteLine($"~~~ Day {_day} (Night) ~~~");
                Console.WriteLine("");

                var infoText = "Night falls. You can yet again roam this land.";
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
                }

                Console.WriteLine(infoText);
                Console.WriteLine("");

                // Stat Report
                Console.WriteLine($"HEALTH: {_you.Hitpoints}");
                Console.WriteLine($"HUNGER LEVEL: {hungerLevel}");
                Console.WriteLine($"LIVING VILLAGERS: {_village.Villagers}");
                Console.WriteLine($"(DEV) VILLAGE SUSPICION: {_village.Suspicion}");
                Console.WriteLine("");

                // Actions
                Console.WriteLine("1. Hide (Default)");
                Console.WriteLine("2. Feed on villager");

                Console.WriteLine("");
                Console.Write(": ");

                var option = Console.ReadLine();
                Console.Clear();
                switch (option)
                {
                    case "2":
                        // try to feed
                        if (Randomizer.Roll(1, 20) >= (10 + Math.Round(10f * _village.Suspicion)))
                        {
                            Console.WriteLine("You feed a villager successfully. You hunger recedes. ...For now");
                            // Feeds
                            _you.LastFed = _day;
                            _village.Villagers--;
                        }
                        else
                        {
                            Console.WriteLine("You fail your attempt to feed on a villager. The village grows more suspicious.");
                            _village.Suspicion = Math.Min(1, _village.Suspicion + ((float)Randomizer.Roll(3, 10) / 100f)); // Can't get more suspicious than 1
                        }
                        break;
                    case "1":
                    default:
                        Console.WriteLine("You hide for the night. The village grows less suspicious.");
                        _village.Suspicion = Math.Max(0, _village.Suspicion - ((float)Randomizer.Roll(3, 10) / 100f)); // Can't get less suspicious than 0 
                        break; 
                    case "q":
                        // TODO: Remove kill game hack
                        _isGameOver = true;
                        break;
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
           
        }

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
