using System;
using JoysOfEfficiency.Core;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using xTile.Layers;
using Object = StardewValley.Object;

namespace JoysOfEfficiency.Automation
{
    internal class TrashCanScavenger
    {
        private static IReflectionHelper Reflection => InstanceHolder.Reflection;
        private static IMonitor Monitor => InstanceHolder.Monitor;

        public static void ScavengeTrashCan()
        {
            if (!(Game1.currentLocation is Town))
            {
                return;
            }

            Farmer player = Game1.player;
            int radius = InstanceHolder.Config.BalancedMode ? 1 : InstanceHolder.Config.ScavengingRadius;
            Layer layer = Game1.currentLocation.Map.GetLayer("Buildings");
            int ox = player.getTileX(), oy = player.getTileY();
            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    int x = ox + dx, y = oy + dy;

                    if (layer.Tiles[x, y]?.TileIndex == 78)
                    {
                        CollectTrashCan(x, y);
                    }
                }
            }
        }
        private static void CollectTrashCan(int x, int y)
        {
            if (!(Game1.currentLocation is Town town))
            {
                return;
            }

            NetArray<bool, NetBool> garbageChecked =
                Reflection.GetField<NetArray<bool, NetBool>>(town, "garbageChecked").GetValue();

            string text = Game1.currentLocation.doesTileHaveProperty(x, y, "Action", "Buildings");
            int num = text != null ? Convert.ToInt32(text.Split(' ')[1]) : -1;
            if (num >= 0 && num < garbageChecked.Length && !garbageChecked[num])
            {
                garbageChecked[num] = true;
                Game1.currentLocation.playSound("trashcan");
                Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + 777 + num);
                if (random.NextDouble() < 0.2 + Game1.dailyLuck)
                {
                    int parentSheetIndex = 168;
                    switch (random.Next(10))
                    {
                        case 0:
                            parentSheetIndex = 168;
                            break;
                        case 1:
                            parentSheetIndex = 167;
                            break;
                        case 2:
                            parentSheetIndex = 170;
                            break;
                        case 3:
                            parentSheetIndex = 171;
                            break;
                        case 4:
                            parentSheetIndex = 172;
                            break;
                        case 5:
                            parentSheetIndex = 216;
                            break;
                        case 6:
                            parentSheetIndex = Utility.getRandomItemFromSeason(Game1.currentSeason, x * 653 + y * 777, false);
                            break;
                        case 7:
                            parentSheetIndex = 403;
                            break;
                        case 8:
                            parentSheetIndex = 309 + random.Next(3);
                            break;
                        case 9:
                            parentSheetIndex = 153;
                            break;
                    }
                    switch (num)
                    {
                        case 3 when random.NextDouble() < 0.2 + Game1.dailyLuck:
                            parentSheetIndex = 535;
                            if (random.NextDouble() < 0.05)
                            {
                                parentSheetIndex = 749;
                            }

                            break;
                        case 4 when random.NextDouble() < 0.2 + Game1.dailyLuck:
                            parentSheetIndex = 378 + random.Next(3) * 2;
                            break;
                        case 5 when random.NextDouble() < 0.2 + Game1.dailyLuck && Game1.dishOfTheDay != null:
                            parentSheetIndex = Game1.dishOfTheDay.ParentSheetIndex != 217 ? Game1.dishOfTheDay.ParentSheetIndex : 216;
                            break;
                        case 6 when random.NextDouble() < 0.2 + Game1.dailyLuck:
                            parentSheetIndex = 223;
                            break;
                    }

                    Monitor.Log($"You picked up trash @ [{x},{y}]");
                    Game1.player.addItemByMenuIfNecessary(new Object(parentSheetIndex, 1));
                }
            }
        }
    }
}
