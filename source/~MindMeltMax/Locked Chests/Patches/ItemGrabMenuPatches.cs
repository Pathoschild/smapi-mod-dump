/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using LockChests.Utilities;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockChests.Patches
{
    internal class ItemGrabMenuPatches
    {
        public static bool receiveLeftClick_prefix(ItemGrabMenu __instance, int x, int y, bool playSound = true)
        {
            try
            {
                if (ModEntry.IHelper.Reflection.GetField<Item>(__instance, "sourceItem").GetValue() is not null and Chest c && c.playerChest.Value)
                {
                    if (c.modData.ContainsKey("LockedChests.Owner") && c.modData.ContainsKey("LockedChests.Lock") && c.modData.ContainsKey("LockedChests.AccessIds"))
                    {
                        long owner = Convert.ToInt64(c.modData["LockedChests.Owner"]);
                        Lock chestLock = Enum.Parse<Lock>(c.modData["LockedChests.Lock"]);
                        List<long> accessIds = Json.Read<List<long>>(c.modData["LockedChests.AccessIds"]) ?? new List<long>() { Game1.player.UniqueMultiplayerID };

                        if (owner == Game1.player.UniqueMultiplayerID || chestLock == Lock.Open || accessIds.Any(x => x == Game1.player.UniqueMultiplayerID)) 
                            return true;
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ModEntry.IMonitor.Log($"Failed patching ItemGrabMenu.receiveLeftClick, allowing {Game1.player.Name} access", LogLevel.Error);
                ModEntry.IMonitor.Log($"{ex} - {ex.Message}\n{ex.StackTrace}", LogLevel.Trace);
                return true;
            }
        }
    }
}
