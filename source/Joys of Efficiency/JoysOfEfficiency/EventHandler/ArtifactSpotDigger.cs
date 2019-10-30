using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace JoysOfEfficiency.EventHandler
{
    internal class ArtifactSpotDigger
    {
        private static Config Config => InstanceHolder.Config;

        public static void DigNearbyArtifactSpots()
        {
            Farmer player = Game1.player;
            int radius = Config.AutoDigRadius;
            Hoe hoe = Util.FindToolFromInventory<Hoe>(player, InstanceHolder.Config.FindHoeFromInventory);
            GameLocation location = player.currentLocation;
            if (hoe == null)
            {
                return;
            }

            bool flag = false;
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    int x = player.getTileX() + i;
                    int y = player.getTileY() + j;
                    Vector2 loc = new Vector2(x, y);
                    if (!location.Objects.ContainsKey(loc) || location.Objects[loc].ParentSheetIndex != 590 ||
                        location.isTileHoeDirt(loc))
                    {
                        continue;
                    }
                    location.digUpArtifactSpot(x, y, player);
                    location.Objects.Remove(loc);
                    location.terrainFeatures.Add(loc, new HoeDirt());
                    flag = true;
                }
            }

            if (flag)
            {
                Game1.playSound("hoeHit");
            }
        }

    }
}
