/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;

namespace HugsAndKisses.Framework
{
    public static class NPCPatches
    {
        private static IMonitor Monitor;
        private static ModConfig Config;
        private static IModHelper Helper;

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor, ModConfig config, IModHelper helper)
        {
            Monitor = monitor;
            Config = config;
            Helper = helper;
        }

        public static bool NPC_checkAction_Prefix(ref NPC __instance, ref Farmer who, GameLocation l, ref bool __result)
        {
            try
            {
                if (!Config.EnableMod || __instance.IsInvisible || __instance.isSleeping.Value || !who.canMove || who.checkForQuestComplete(__instance, -1, -1, who.ActiveObject, null, -1, 5) || who.pantsItem.Value?.ParentSheetIndex == 15 && (__instance.Name.Equals("Lewis") || __instance.Name.Equals("Marnie")) || __instance.Name.Equals("Krobus") && who.hasQuest("28") || !who.IsLocalPlayer)
                {
                    return true;
                }

                Monitor.Log($"Checking action for {who.Name} kissing/hugging {__instance.Name}", LogLevel.Debug);
                if (!who.friendshipData.TryGetValue(__instance.Name, out var data))
                {
                    Monitor.Log($"Checking action failed, {__instance.Name} is missing relation data.", LogLevel.Debug);
                    return true;
                }

                if (!data.IsMarried() && !data.IsEngaged() && !((__instance.datable.Value || Config.AllowNonDateableNPCsToHugAndKiss) && ((data.IsDating() && Config.DatingKisses) || (who.getFriendshipHeartLevelForNPC(__instance.Name) >= Config.HeartsForFriendship && Config.FriendHugs))))
                {
                    Monitor.Log($"Checking action failed, config disallow it. married = {data.IsMarried()}, engaged = {data.IsEngaged()}, dateable = {__instance.datable.Value}, dating = {data.IsDating()}, hearts = {who.getFriendshipHeartLevelForNPC(__instance.Name)}", LogLevel.Debug);
                    return true;
                }

                __instance.faceDirection(-3);
                if (__instance.Sprite.CurrentAnimation is not null)
                {
                    Monitor.Log($"Checking action failed, {__instance.Name} is in an animation.", LogLevel.Debug);
                    return true;
                }

                if (__instance.hasTemporaryMessageAvailable())
                {
                    Monitor.Log($"Checking action failed, {__instance.Name} has temporary message available.", LogLevel.Debug);
                    return true;
                }

                if (__instance.currentMarriageDialogue.Count > 0 || __instance.CurrentDialogue.Count > 0)
                {
                    Monitor.Log($"Checking action failed, {__instance.Name} has dialoge available.", LogLevel.Debug);
                    return true;
                }

                if (__instance.isMoving())
                {
                    Monitor.Log($"Checking action failed, {__instance.Name} is moving.", LogLevel.Debug);
                    return true;
                }

                if (who.ActiveObject is not null)
                {
                    Monitor.Log($"Checking action failed, {__instance.Name} is holding an object.", LogLevel.Debug);
                    return true;
                }

                bool kissing = data.IsDating() || data.IsMarried() || data.IsEngaged();
                if (kissing && __instance.hasBeenKissedToday.Value && !Config.UnlimitedDailyKisses)
                {
                    Monitor.Log($"Kissing failed, already kissed {__instance.Name}", LogLevel.Debug);
                    return true;
                }

                string actionName = kissing ? "kissing" : "hugging";
                __instance.faceGeneralDirection(who.getStandingPosition(), 0, false);
                who.faceGeneralDirection(__instance.getStandingPosition(), 0, false);
                if (__instance.FacingDirection != 3 && __instance.FacingDirection != 1)
                {
                    Monitor.Log($"{actionName} failed, {__instance.Name} is facing the wrong direction", LogLevel.Debug);
                    return true;
                }

                Monitor.Log($"{who.Name} {actionName} {__instance.Name}", LogLevel.Debug);
                if (kissing)
                {
                    Kissing.PlayerNPCKiss(who, __instance);
                }
                else
                {
                    Kissing.PlayerNPCHug(who, __instance);
                }

                __result = true;
                return false;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(NPC_checkAction_Prefix)}:\n{ex}", LogLevel.Error);
            }
            return true;
        }
    }
}
