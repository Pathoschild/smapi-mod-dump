/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Utils.Extensions;
using GingerIslandMainlandAdjustments.ScheduleManager;
using HarmonyLib;
using StardewModdingAPI.Utilities;
using StardewValley.Locations;

namespace GingerIslandMainlandAdjustments.DialogueChanges;

/// <summary>
/// Class to handle patching of NPCs for dialogue.
/// </summary>
[HarmonyPatch(typeof(NPC))]
internal static class DialoguePatches
{
    private const string ANTISOCIAL = "Resort_Antisocial";
    private const string ISLANDNORTH = "Resort_IslandNorth";
    private const string TOADVENTURE = "Resort_Adventure";
    private const string FROMADVENTURE = "Resort_AdventureReturn";

    private static readonly PerScreen<HashSet<string>> TalkedToTodayPerScreen = new(createNewState: () => new HashSet<string>());

    private static HashSet<string> TalkedToToday => TalkedToTodayPerScreen.Value;

    /// <summary>
    /// Clears the record of whether or not you've talked to your spouse on the Island today.
    /// </summary>
    internal static void ClearTalkRecord() => TalkedToToday.Clear();

    /// <summary>
    /// Appends checkForNewCurrentDialogue to look for GI-specific dialogue.
    /// </summary>
    /// <param name="__instance">NPC instance.</param>
    /// <param name="__0">Heart level.</param>
    /// <param name="__1">Whether or not to have a season prefix.</param>
    /// <param name="__result">Whether or not new dialogue has been found.</param>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(NPC.checkForNewCurrentDialogue))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Convention used by Harmony")]
    private static void DoCheckIslandDialogue(NPC __instance, int __0, bool __1, ref bool __result)
    { // __0 = heartlevel, as int. __1 = whether or not to have a season prefix?
        try
        {
            if (__instance.currentLocation is IslandLocation)
            {
                TalkedToToday.Add(__instance.Name);
            }
            if (__result || !Game1.IsVisitingIslandToday(__instance.Name) || __instance.currentLocation is FarmHouse)
            {
                return;
            }
            if (__instance.currentLocation is IslandLocation && GIScheduler.CurrentAdventurers?.Contains(__instance) == true)
            {
                if (Game1.timeOfDay < 1200 && __instance.Dialogue.ContainsKey(TOADVENTURE))
                {
                    __instance.ClearAndPushDialogue(TOADVENTURE);
                    return;
                }
                else if (Game1.timeOfDay > 1700 && __instance.Dialogue.ContainsKey(FROMADVENTURE))
                {
                    __instance.ClearAndPushDialogue(FROMADVENTURE);
                    return;
                }
            }
            if (__instance.currentLocation is IslandEast && __instance.Dialogue.ContainsKey(ANTISOCIAL))
            {
                __instance.ClearAndPushDialogue(ANTISOCIAL);
                return;
            }
            else if (__instance.currentLocation is IslandNorth && __instance.Dialogue.ContainsKey(ISLANDNORTH))
            {
                __instance.ClearAndPushDialogue(ISLANDNORTH);
                return;
            }
            else if (__instance.currentLocation is IslandLocation)
            {
                return;
            }

            string preface = __1 ? string.Empty : Game1.currentSeason;

            string baseKey;

            if (Game1.timeOfDay <= 1200)
            {
                baseKey = preface + "Resort_Approach";
            }
            else if (Game1.timeOfDay >= 1800)
            {
                baseKey = preface + "Resort_Left";
                if (!__instance.currentLocation.IsOutdoors && __instance.currentLocation is not FishShop)
                {
                    baseKey = $"{baseKey}_{__instance.currentLocation.Name}"; // use specific INDOOR keys.
                }
            }
            else
            {
                return;
            }

            // Handle group-specific dialogue.
            if (GIScheduler.CurrentGroup is not null
                && GIScheduler.CurrentVisitingGroup?.Contains(__instance) == true
                && DialogueUtilities.TryGetIslandDialogue(__instance, $"{baseKey}_{GIScheduler.CurrentGroup}", __0))
            {
                __result = true;
                return;
            }

            if (__instance.getSpouse() is Farmer spouse && spouse == Game1.player
                && DialogueUtilities.TryGetIslandDialogue(__instance, baseKey + "_marriage", __0))
            {
                __result = true;
                return;
            }
            __result = DialogueUtilities.TryGetIslandDialogue(__instance, baseKey, __0);
            return;
        }
        catch (Exception ex)
        {
            Globals.ModMonitor.Log($"Error in checking for island dialogue for NPC {__instance.Name}\n{ex}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Appends spouse arrival back at farmhouse to replace with GI-specific dialogue.
    /// </summary>
    /// <param name="__instance">NPC instance.</param>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(NPC.arriveAtFarmHouse))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Convention used by Harmony")]
    private static void AppendArrival(NPC __instance)
    {
        try
        {
            if (!Game1.IsVisitingIslandToday(__instance.Name))
            {
                return;
            }
            if (TalkedToToday.Contains(__instance.Name) && __instance.TryApplyMarriageDialogueIfExisting("GIReturn_Talked_" + __instance.Name, clearOnMovement: true))
            {
                Globals.ModMonitor.DebugOnlyLog($"Setting GIReturn_Talked_{__instance.Name}.", LogLevel.Debug);
            }
            else if (__instance.TryApplyMarriageDialogueIfExisting("GIReturn_" + __instance.Name, clearOnMovement: true))
            {
                Globals.ModMonitor.DebugOnlyLog($"Setting GIReturn_{__instance.Name}.", LogLevel.Debug);
            }
            else
            {
                __instance.CurrentDialogue.Clear();
                __instance.currentMarriageDialogue.Clear();
                if (Game1.player.getFriendshipHeartLevelForNPC(__instance.Name) > 9)
                {
                    __instance.CurrentDialogue.Push(new Dialogue(I18n.GILeaveDefaultHappy(__instance.getTermOfSpousalEndearment()), __instance));
                }
                else
                {
                    __instance.CurrentDialogue.Push(new Dialogue(I18n.GILeaveDefaultUnhappy(), __instance));
                }
            }
        }
        catch (Exception ex)
        {
            Globals.ModMonitor.Log($"Error in setting GIReturn dialogue for {__instance.Name}:\n{ex}", LogLevel.Error);
        }
    }
}

/// <summary>
/// Class that holds patches against IslandSouth, for dialogue.
/// </summary>
[HarmonyPatch(typeof(Game1))]
internal class Game1DialoguePatches
{
    /// <summary>
    /// Postfix SetupIslandSchedules to add marriage-specific dialogue.
    /// </summary>
    /// <remarks>DayStarted, unfortunately, runs *before* SetupIslandSchedules.</remarks>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Game1.updateWeatherIcon))]
    private static void AppendMarriageDialogue()
    {
        if (!Game1.newDay && Game1.gameMode != Game1.loadingMode)
        {
            return;
        }

        try
        {
            if (Game1.player?.getSpouse() is NPC spouse && Game1.IsVisitingIslandToday(spouse.Name))
            {
                if (spouse.TryApplyMarriageDialogueIfExisting("GILeave_" + spouse.Name))
                {
#if DEBUG
                Globals.ModMonitor.Log($"Setting GILeave_{spouse?.Name}", LogLevel.Trace);
#endif
                }
                else if (Game1.player is not null)
                {
                    spouse.CurrentDialogue.Clear();
                    spouse.currentMarriageDialogue.Clear();
                    if (Game1.player.getFriendshipHeartLevelForNPC(spouse.Name) > 9)
                    {
                        spouse.CurrentDialogue.Push(new Dialogue(I18n.GILeaveDefaultHappy(spouse.getTermOfSpousalEndearment()), spouse));
                    }
                    else
                    {
                        spouse.CurrentDialogue.Push(new Dialogue(I18n.GILeaveDefaultUnhappy(), spouse));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Globals.ModMonitor.Log($"Error in setting GILeave dialogue:\n{ex}", LogLevel.Error);
        }
    }
}