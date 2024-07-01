/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using ChestDisplays.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using System;
using System.Linq;

namespace ChestDisplays.Patches
{
    public class ChestPatches
    {
        private static IModHelper Helper => ModEntry.IHelper;
        private static IMonitor Monitor => ModEntry.IMonitor;

        public static void draw_postfix(Chest __instance, SpriteBatch spriteBatch, int x, int y)
        {
            try
            {
                if (Utils.InvalidChest(__instance)) 
                    return;
                int lidFrame = Helper.Reflection.GetField<int>(__instance, "currentLidFrame").GetValue();
                if (lidFrame != __instance.startingLidFrame.Value) 
                    return;

                int itemType = -1;
                bool isBigChest = __instance.SpecialChestType == Chest.SpecialChestTypes.BigChest;
                bool isMiniFridge = __instance.QualifiedItemId == "(BC)216";
                if (!Utils.displayItemsCache.TryGetValue(__instance, out Item? i))
                {
                    if (__instance.Items.HasAny() && ModEntry.IConfig.ShowFirstIfNoneSelected)
                    {
                        i = __instance.Items.FirstOrDefault(x => x is not null);
                        if (i is null)
                            return;
                        itemType = Utils.getItemType(i);
                        Utils.drawItem(spriteBatch, i, itemType, Utils.GetLocationFromItemType(itemType, x, y, isBigChest, isMiniFridge), Utils.GetDepthFromItemType(itemType, x, y));
                    }
                }
                else
                {
                    itemType = Utils.getItemType(i);
                    Utils.drawItem(spriteBatch, i, itemType, Utils.GetLocationFromItemType(itemType, x, y, isBigChest, isMiniFridge), Utils.GetDepthFromItemType(itemType, x, y));
                }
                var loc = Utils.GetLocationFromItemType(1, x, y, isBigChest, isMiniFridge);
                if (new Rectangle((int)loc.X, (int)loc.Y - 1, 64, 96).Contains(Game1.getMousePosition()) && Utils.displayModDataCache.TryGetValue(__instance, out var data) && !string.IsNullOrWhiteSpace(data.Name))
                    SpriteText.drawSmallTextBubble(spriteBatch, data.Name, loc + new Vector2(32, -32), 256, (float)(0.98000001907348633 + __instance.TileLocation.X / 64 * 9.9999997473787516E-05 + __instance.TileLocation.X / 64 * 9.9999999747524271E-07));
            }
            catch(Exception ex)
            {
                Monitor.LogOnce($"Failed drawing object at X : {x} - Y : {y}", LogLevel.Error);
                Monitor.LogOnce($"{ex.GetType().FullName} - {ex.Message}\n{ex.StackTrace}", LogLevel.Trace); 
            }
        }
    }
}
