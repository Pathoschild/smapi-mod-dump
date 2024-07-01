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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Events;
public class EventManager
{
    public readonly Event<WateringFinishedArgs> FinishedWateringSoil;
    public readonly Event TestEvent;

    public EventManager()
    {
        FinishedWateringSoil = new();
        TestEvent = new();
    }

    internal static void InvokeEvent<T>(EventHandler<T> handler, object sender, T eventArg)
    {
        if (handler is null)
            return;

        foreach (var handle in handler.GetInvocationList().Cast<EventHandler<T>>())
        {
            try
            {
                handle.Invoke(sender, eventArg);
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Something went wrong when handling event {handle} ({sender}):\n{e}", StardewModdingAPI.LogLevel.Error);
            }
        }
    }

    internal static void InvokeEvent<T>(EventHandler<T> event_handler, T event_args)
    {
        InvokeEvent<T>(event_handler, null, event_args);
    }
}
