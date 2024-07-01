/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using System;
using System.Linq;

namespace AchtuurCore.Events;

public static class EventPublisher
{
    internal static void InvokeEvent(EventHandler handler, object sender)
    {
        if (handler is null)
            return;

        foreach (var handle in handler.GetInvocationList().Cast<EventHandler>())
        {
            try
            {
                handle.Invoke(sender, new EventArgs());
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Something went wrong when handling event {handle} ({sender}):\n{e}", StardewModdingAPI.LogLevel.Error);
            }
        }
    }

    
}
