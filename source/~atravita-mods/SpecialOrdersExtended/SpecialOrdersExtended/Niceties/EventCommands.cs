/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraCore.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace SpecialOrdersExtended.Niceties;

/// <summary>
/// Holds event commands.
/// </summary>
[HarmonyPatch(typeof(Event))]
internal static class EventCommands
{
    /// <summary>
    /// The name for the event command to add a special order.
    /// </summary>
    internal const string ADD_SPECIAL_ORDER = "atravita_addSpecialOrder";

    /// <summary>
    /// Adds aa special order in an event coommand.
    /// </summary>
    /// <param name="event">The event instance.</param>
    /// <param name="location">GameLocation.</param>
    /// <param name="time">time.</param>
    /// <param name="split">evvent command, split.</param>
    /// <remarks>This call pattern is required to register events with spacecore.</remarks>
    internal static void AddSpecialOrder(Event @event, GameLocation location, GameTime time, string[] split)
    {
        if (split.Length == 2)
        {
            try
            {
                SpecialOrder order = SpecialOrder.GetSpecialOrder(split[1], Game1.random.Next());
                Game1.player.team.specialOrders.Add(order);
                MultiplayerHelpers.GetMultiplayer().globalChatInfoMessage("AcceptedSpecialOrder", Game1.player.Name, order.GetName());
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"Mod failed while attempting to adding a special order:\n\n{ex}", LogLevel.Error);
            }
        }
        else
        {
            ModEntry.ModMonitor.Log($"Command {ADD_SPECIAL_ORDER} expects a single argument, the internal name of the order", LogLevel.Warn);
        }

        @event.CurrentCommand++;
        @event.checkForNextCommand(location, time);
    }

    /// <summary>
    /// Prefixes the TryGetCommand function to add in our event command.
    /// </summary>
    /// <param name="__instance">event.</param>
    /// <param name="location">location the event is at.</param>
    /// <param name="time">game time.</param>
    /// <param name="split">the event command, split.</param>
    /// <returns>True to continue to original function, false otherwise.</returns>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    internal static bool PrefixTryGetCommand(Event __instance, GameLocation location, GameTime time, string[] split)
    {
        if (split.Length != 2)
        {
            return true;
        }
        else if (split[0].Equals(ADD_SPECIAL_ORDER, StringComparison.Ordinal))
        {
            AddSpecialOrder(__instance, location, time, split);
            return false;
        }
        return true;
    }
}