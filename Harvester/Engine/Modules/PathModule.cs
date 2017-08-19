using Harvester.Engine.Loaders;
using System.Collections.Generic;
using System.Linq;
using ZzukBot.Game.Statics;
using ZzukBot.Objects;

namespace Harvester.Engine.Modules
{
    public class PathModule
    {
        private Navigation Navigation { get; }
        private ObjectManager ObjectManager { get; }
        private ProfileLoader ProfileLoader { get; }

        public PathModule(Navigation navigation, ObjectManager objectManager, ProfileLoader profileLoader)
        {
            Navigation = navigation;
            ObjectManager = objectManager;
            ProfileLoader = profileLoader;
        }

        public int index = -1;
        public List<string> playerPositions = new List<string> { };

        public Location Path(Location destination)
        {
            LocalPlayer player = ObjectManager.Player;
            Location playerPos = player.Position;

            Location[] pathArray = Navigation.CalculatePath(player.MapId, playerPos, destination, true);
            List<Location> pathList = pathArray.ToList();
            Location closestWaypoint = pathList.OrderBy(x => playerPos.GetDistanceTo(x)).First();
            int index = pathList.FindIndex(x => x.Equals(closestWaypoint)) + 1;

            return pathList[index];
        }

        public void Traverse(Location destination)
        {
            LocalPlayer player = ObjectManager.Player;

            player.CtmTo(Path(destination));
        }

        public Location GetNextHotspot()
        {
            LocalPlayer player = ObjectManager.Player;
            Location playerPos = player.Position;

            if (index == -1)
            {
                Location closestHotspot = ProfileLoader.hotspots.OrderBy(x => playerPos.GetDistanceTo(x)).First();
                index = ProfileLoader.hotspots.FindIndex(x => x.Equals(closestHotspot));
            }

            if (playerPos.GetDistanceTo(ProfileLoader.hotspots[index]) < 2)
                index++;

            if (index >= ProfileLoader.hotspots.Count())
                index = 0;

            return ProfileLoader.hotspots[index];
        }

        public Location GetNextVendorHotspot()
        {
            LocalPlayer player = ObjectManager.Player;
            Location playerPos = player.Position;

            if (index == -1)
            {
                Location closestHotspot = ProfileLoader.vendor.OrderBy(x => playerPos.GetDistanceTo(x)).First();
                index = ProfileLoader.vendor.FindIndex(x => x.Equals(closestHotspot));
            }

            if (playerPos.GetDistanceTo(ProfileLoader.vendor[index]) < 2)
                index++;

            if (index >= ProfileLoader.vendor.Count())
                index = 0;

            return ProfileLoader.vendor[index];
        }

        public bool Stuck()
        {
            if (playerPositions.FindAll(x => x.Equals(playerPositions.Last())).Count() >= 10)
                return true;

            return false;
        }
    }
}