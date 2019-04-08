using Count.Models;

/// <summary>
/// PARKED for now
/// </summary>
namespace Count.Controllers
{
    public class PlotController
    {
        private Plot _plot { get; set; }

        public PlotController()
        {
            _plot = new Plot
            {
                Name = "Eternal love",
                Description = "Kidnap the mayor's daughter",
                Duration = 2,
                Reward = 5,
                FollowersRequired = 2,
                FollowersMinLevel = 0
            };
        }

        public void Start()
        {

        }
    }
}
