/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Chest_Displays.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using SObject = StardewValley.Object;
using SUtils = StardewValley.Utility;

namespace Chest_Displays.Harmony
{
    public class ChestPatches
    {
        private static IModHelper Helper = ModEntry.RequestableHelper;
        private static IMonitor Monitor = ModEntry.RequestableMonitor;
        private static Config Config = ModEntry.RequestableConfig;

        public static void draw_postfix(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            try
            {
                foreach (SaveData sd in ModEntry.SavedData)
                {
                    if(sd.Location == Game1.player.currentLocation.Name && sd.X == x && sd.Y == y)
                    {
                        Chest current = Game1.player.currentLocation.getObjectAtTile(x, y) as Chest;
                        if(current != null)
                        {
                            int lidFrame = Helper.Reflection.GetField<int>(current, "currentLidFrame").GetValue();
                            if (lidFrame != current.startingLidFrame) continue;
                        }
                        Item i;
                        if (ModEntry.RequestableConfig.RetainItem)
                            i = SUtils.getItemFromStandardTextDescription(sd.ItemDescription, null);
                        else
                            i = Utils.getItemFromName(sd.Item, current);
                        if (i == null)
                            continue;
                        int itemType = Utils.getItemType(i);
                        if (itemType == 1) (i as SObject).Quality = sd.ItemQuality;
                        Utils.drawItem(spriteBatch, i, itemType, x, y, Utils.GetLocationFromItemType(itemType, x, y), Utils.GetDepthFromItemType(itemType, x, y));
                        return;
                    }
                }

                foreach(var o in Game1.player.currentLocation.Objects.Values)
                {
                    if (o is Chest && o.TileLocation == new Vector2(x, y))
                    {
                        Chest current = o as Chest;
                        if (current != null)
                        {
                            if (Utils.nullChest(current) || current.items.Count <= 0)
                                continue;
                            int lidFrame = Helper.Reflection.GetField<int>(current, "currentLidFrame").GetValue();
                            if (lidFrame != current.startingLidFrame) continue;
                            Item i = current.items[0];
                            if (i == null)
                                continue;
                            int itemType = Utils.getItemType(i);
                            Utils.drawItem(spriteBatch, i, itemType, x, y, Utils.GetLocationFromItemType(itemType, x, y), Utils.GetDepthFromItemType(itemType, x, y));
                            return;
                        }
                    }
                }
            }
            catch(Exception ex) { Monitor.Log($"Failed drawing object at X : {x} - Y : {y}", LogLevel.Error);  Monitor.Log($"{ex} - {ex.Message}", LogLevel.Trace); }
        }
    }
}
