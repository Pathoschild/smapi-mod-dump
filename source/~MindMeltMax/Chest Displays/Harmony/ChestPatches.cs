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
using Newtonsoft.Json;
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

namespace ChestDisplays.Patches
{
    public class ChestPatches
    {
        private static IModHelper Helper => ModEntry.IHelper;
        private static IMonitor Monitor => ModEntry.IMonitor;

        public static void draw_postfix(Chest __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            try
            {
                if (Utils.InvalidChest(__instance)) 
                    return;
                int lidFrame = Helper.Reflection.GetField<int>(__instance, "currentLidFrame").GetValue();
                if (lidFrame != __instance.startingLidFrame.Value) 
                    return;

                Item? i = null;
                int itemType = -1;
                if (!Utils.displayItemsCache.ContainsKey(__instance) && __instance.Items.HasAny())
                {
                    i = __instance.Items.FirstOrDefault(x => x is not null);
                    if (i is null)
                        return;
                    itemType = Utils.getItemType(i);
                    Utils.drawItem(spriteBatch, i, itemType, Utils.GetLocationFromItemType(itemType, x, y), Utils.GetDepthFromItemType(itemType, x, y));
                    return;
                }
                i = Utils.displayItemsCache[__instance];
                itemType = Utils.getItemType(i);
                Utils.drawItem(spriteBatch, i, itemType, Utils.GetLocationFromItemType(itemType, x, y), Utils.GetDepthFromItemType(itemType, x, y));
                /*if (__instance.modData.ContainsKey(ModEntry.IHelper.ModRegistry.ModID))
                {
                    var data = JsonConvert.DeserializeObject<ModData>(__instance.modData[Helper.ModRegistry.ModID]);
                    if (data is not null)
                    {
                        Item? i = Utils.getItemFromName(data.Item, data.ItemType, data.ItemQuality, data.UpgradeLevel, data.Color);
                        if (i is null) 
                            flag1 = true;
                        else 
                            Utils.drawItem(spriteBatch, i, data.ItemType, x, y, Utils.GetLocationFromItemType(data.ItemType, x, y), Utils.GetDepthFromItemType(data.ItemType, x, y));
                    }
                }
                else 
                    flag1 = true;

                if (flag1 && Config.ShowFirstIfNoneSelected && __instance.Items.Count > 0)
                {
                    Item? i = __instance.Items.FirstOrDefault(x => x is not null);
                    if (i is null) 
                        return;
                    int itemType = Utils.getItemType(i);
                    Utils.drawItem(spriteBatch, i, itemType, x, y, Utils.GetLocationFromItemType(itemType, x, y), Utils.GetDepthFromItemType(itemType, x, y));
                }*/
            }
            catch(Exception ex) { Monitor.LogOnce($"Failed drawing object at X : {x} - Y : {y}", LogLevel.Error);  Monitor.LogOnce($"{ex.GetType().FullName} - {ex.Message}", LogLevel.Trace); }
        }

        
    }
}
