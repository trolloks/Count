using Count.Models;
using Count.Models.LocationObjects.Structures.Neutral;

namespace Count.Controllers.LocationObjects.Structures.Neutral
{
    public class ForestController : NeutralLocationController
    {
        public ForestController(Location worldLocation) : base(worldLocation)
        {
            _object = new Forest()
            {
                Name = "Forest",
                Description = string.Empty,
                WorldLocation = worldLocation
            };
        }

        public override bool Upkeep(Game game)
        {
            return false;
        }
    }
}
