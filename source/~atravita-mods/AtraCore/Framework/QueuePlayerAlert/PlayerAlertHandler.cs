/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using Microsoft.Toolkit.Diagnostics;
using StardewModdingAPI.Utilities;

namespace AtraCore.Framework.QueuePlayerAlert;

/// <summary>
/// Handles queuing alerts to the player.
/// </summary>
public static class PlayerAlertHandler
{
    private static readonly PerScreen<Queue<HUDMessage>> QueuedMessages = new(() => new());

    public static void AddMessage(HUDMessage message)
    {
        Guard.IsNotNull(message, nameof(message));

        QueuedMessages.Value.Enqueue(message);
    }

    /// <summary>
    /// Called every ten-in game minutes - loads up to three messages to the player.
    /// </summary>
    internal static void DisplayFromQueue()
    {
        int i = 0;
        while (QueuedMessages.Value.TryDequeue(out HUDMessage? message) && ++i < 3)
        {
            Game1.addHUDMessage(message);
        }
    }
}
