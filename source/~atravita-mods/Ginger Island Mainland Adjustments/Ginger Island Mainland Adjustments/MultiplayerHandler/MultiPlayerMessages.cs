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
using GingerIslandMainlandAdjustments.AssetManagers;
using HarmonyLib;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace GingerIslandMainlandAdjustments.MultiplayerHandler;

/// <summary>
/// Class to handle multiplayer shared state.
/// </summary>
[HarmonyPatch(typeof(NPC))]
public static class MultiplayerSharedState
{
    private const string SCHEDULEMESSAGE = "GIMAScheduleUpdateMessage";

    /// <summary>
    /// Gets or sets pam's current schedule string.
    /// </summary>
    internal static string? PamsSchedule { get; set; }

    /// <summary>
    /// Updates entry for Pam's schedule whenever a person joins in multiplayer.
    /// </summary>
    /// <param name="e">arguments.</param>
    internal static void ReSendMultiplayerMessage(PeerConnectedEventArgs e)
    {
        if (Context.IsMainPlayer && Context.IsWorldReady
            && Game1.getCharacterFromName("Pam") is NPC pam
            && pam.TryGetScheduleEntry(pam.dayScheduleName.Value, out string? rawstring)
            && Globals.UtilitySchedulingFunctions.TryFindGOTOschedule(pam, SDate.Now(), rawstring, out string redirectedstring))
        {
            PamsSchedule = redirectedstring;
            Globals.ModMonitor.Log($"Grabbing Pam's rawSchedule for phone: {redirectedstring}");
            Globals.Helper.Multiplayer.SendMessage(redirectedstring, SCHEDULEMESSAGE, modIDs: new[] { Globals.Manifest.UniqueID }, playerIDs: new[] { e.Peer.PlayerID });
        }
    }

    /// <summary>
    /// Updates entry for Pam's schedule from a multiplayer message.
    /// </summary>
    /// <param name="e">arguments.</param>
    internal static void UpdateFromMessage(ModMessageReceivedEventArgs e)
    {
        if (e.FromModID == Globals.Manifest.UniqueID && e.Type == SCHEDULEMESSAGE)
        {
            PamsSchedule = e.ReadAs<string>();
            Globals.ModMonitor.Log($"Recieved Pam's schedule {PamsSchedule}");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NPC.parseMasterSchedule))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void PostfixGetMasterSchedule(NPC __instance)
    {
        try
        {
            if (Context.IsMainPlayer && __instance?.Name.Equals("Pam", StringComparison.OrdinalIgnoreCase) == true
                && Game1.player.eventsSeen.Contains(AssetEditor.PAMEVENT)
                && __instance.TryGetScheduleEntry(__instance.dayScheduleName.Value, out string? rawstring)
                && Globals.UtilitySchedulingFunctions.TryFindGOTOschedule(__instance, SDate.Now(), rawstring, out string redirectedstring))
            {
                PamsSchedule = redirectedstring;
                Globals.ModMonitor.Log($"Grabbing Pam's rawSchedule for phone: {redirectedstring}");
                Globals.Helper.Multiplayer.SendMessage(redirectedstring, SCHEDULEMESSAGE, modIDs: new[] { Globals.Manifest.UniqueID });
            }
        }
        catch (Exception ex)
        {
            Globals.ModMonitor.Log($"Error in postfixing get master schedule to get Pam's schedule.\n\n{ex}", LogLevel.Error);
        }
    }
}