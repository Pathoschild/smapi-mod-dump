/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using GingerIslandMainlandAdjustments.ScheduleManager;
using GingerIslandMainlandAdjustments.Utils;
using HarmonyLib;
using StardewValley.Locations;

namespace GingerIslandMainlandAdjustments.DialogueChanges;

/// <summary>
/// Class to handle patching of NPCs for dialogue.
/// </summary>
[HarmonyPatch(typeof(NPC))]
internal class DialoguePatches
{
    private const string ANTISOCIAL = "Resort_Antisocial";
    private const string ISLANDNORTH = "Resort_IslandNorth";

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
    public static void DoCheckIslandDialogue(NPC __instance, int __0, bool __1, ref bool __result)
    { // __0 = heartlevel, as int. __1 = whether or not to have a season prefix?
        try
        {
            if (__result)
            { // game code has returned a value, therefore skip me.
                return;
            }
            if (!Game1.IsVisitingIslandToday(__instance.Name))
            { // am not headed to island today.
                return;
            }
            if (__instance.currentLocation is (IslandLocation or FarmHouse))
            { // Currently on island or is spouse in Farmhouse, handle IslandNorth/IslandSoutheast
                if (__instance.currentLocation is IslandEast && __instance.Dialogue.ContainsKey(ANTISOCIAL))
                {
                    __instance.ClearAndPushDialogue(ANTISOCIAL);
                }
                else if (__instance.currentLocation is IslandNorth && __instance.Dialogue.ContainsKey(ISLANDNORTH))
                {
                    __instance.ClearAndPushDialogue(ISLANDNORTH);
                }
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
                    baseKey += __instance.currentLocation.Name; // use specific INDOOR keys.
                }
            }
            else
            {
                return;
            }

            // Handle group-specific dialogue.
            if (GIScheduler.CurrentVisitingGroup?.Contains(__instance) == true
                && GIScheduler.CurrentGroup is not null
                && DialogueUtilities.TryGetIslandDialogue(__instance, $"{baseKey}_{GIScheduler.CurrentGroup}", __0))
            {
                __result = true;
                return;
            }

            Farmer spouse = __instance.getSpouse();
            if (spouse is not null && spouse == Game1.player)
            {
                if (DialogueUtilities.TryGetIslandDialogue(__instance, baseKey + "_marriage", __0))
                {
                    __result = true;
                    return;
                }
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
    public static void AppendArrival(NPC __instance)
    {
        try
        {
            if (!Game1.IsVisitingIslandToday(__instance.Name))
            {
                return;
            }
            __instance.currentMarriageDialogue.Clear();
            __instance.setNewDialogue("MarriageDialogue", "GIReturn_", -1, add: false, clearOnMovement: true);
            Globals.ModMonitor.Log($"Setting GIReturn_{__instance.Name}.", LogLevel.Debug);
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
[HarmonyPatch(typeof(IslandSouth))]
internal class IslandSouthDialoguePatches
{
    /// <summary>
    /// Postfix SetupIslandSchedules to add marriage-specific dialogue.
    /// </summary>
    /// <remarks>DayStarted, unfortunately, runs *before* SetupIslandSchedules.</remarks>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(IslandSouth.SetupIslandSchedules))]
    public static void AppendMarriageDialogue()
    {
        try
        {
            NPC? spouse = Game1.player?.getSpouse();
            if (spouse is not null && Game1.IsVisitingIslandToday(spouse.Name))
            {
                spouse.currentMarriageDialogue.Clear();
                spouse.setNewDialogue("MarriageDialogue", "GILeave_", -1, add: false, clearOnMovement: true);
#if DEBUG
                Globals.ModMonitor.Log($"Setting GILeave_{spouse?.Name}", LogLevel.Trace);
#endif
            }
        }
        catch (Exception ex)
        {
            Globals.ModMonitor.Log($"Error in setting GILeave dialogue:\n{ex}", LogLevel.Error);
        }
    }
}