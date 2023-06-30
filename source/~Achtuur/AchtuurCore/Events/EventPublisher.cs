/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using System;
using System.Linq;

namespace AchtuurCore.Events;

public static class EventPublisher
{
    /// <summary>
    /// This event fires when a player waters an unwatered hoe'd tile.
    /// </summary>
    public static event EventHandler<WateringFinishedArgs> FinishedWateringSoil;

    private static void InvokeEvent(EventHandler handler, object sender)
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

    private static void InvokeEvent<T>(EventHandler<T> handler, object sender, T eventArg)
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


    internal static void InvokeFinishedWateringSoil(object sender, WateringFinishedArgs e)
    {
        EventPublisher.InvokeEvent<WateringFinishedArgs>(FinishedWateringSoil, sender, e);
    }
}
