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
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockChests.Patches
{
    internal class ChestPatches
    {
        public static bool ShowMenu_prefix(Chest __instance)
        {
            try
            {
                if (!__instance.playerChest.Value) return true;

                if (!__instance.modData.ContainsKey("LockedChests.Owner") || !__instance.modData.ContainsKey("LockedChests.Lock") || !__instance.modData.ContainsKey("LockedChests.AccessIds"))
                {
                    __instance.modData.Add("LockedChests.Owner", $"{Game1.player.UniqueMultiplayerID}");
                    __instance.modData.Add("LockedChests.Lock", $"{Lock.Open}");
                    __instance.modData.Add("LockedChests.AccessIds", $"[{Game1.player.UniqueMultiplayerID}]");
                    return true;
                }

                long owner = Convert.ToInt64(__instance.modData["LockedChests.Owner"]);
                Lock chestLock = Enum.Parse<Lock>(__instance.modData["LockedChests.Lock"]);
                List<long> accessIds = Json.Read<List<long>>(__instance.modData["LockedChests.AccessIds"]) ?? new List<long>() { Game1.player.UniqueMultiplayerID };

                if (owner == Game1.player.UniqueMultiplayerID || chestLock != Lock.Locked || accessIds.Any(x => x == Game1.player.UniqueMultiplayerID)) 
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                ModEntry.IMonitor.Log($"Failed patching Chest.checkForAction, lock will not be functional", LogLevel.Error);
                ModEntry.IMonitor.Log($"{ex} - {ex.Message}\n{ex.StackTrace}", LogLevel.Trace);
                return true;
            }
        }
    }
}
