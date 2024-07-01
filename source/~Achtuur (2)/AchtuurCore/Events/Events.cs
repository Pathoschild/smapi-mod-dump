/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Events;
using StardewModdingAPI.Framework.ModLoading.Rewriters.StardewValley_1_6;
using StardewValley.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore;

public class Event<TEventArgs>
{
    private readonly List<EventHandler<TEventArgs>> handlers = new();

    public Event()
    {
        this.handlers = new();
    }

    public void Add(EventHandler<TEventArgs> handler)
    {
        handlers.Add(handler);
    }

    public void Remove(EventHandler<TEventArgs> handler)
    {
        handlers.Remove(handler);
    }

    public void Invoke(object sender, TEventArgs args)
    {
        foreach (var handler in handlers)
        {
            handler.Invoke(sender, args);
        }
    }
}

public class Event
{
    private readonly List<EventHandler> handlers = new();

    public Event()
    {
        this.handlers = new();
    }

    public void Add(EventHandler handler)
    {
        handlers.Add(handler);
    }

    public void Remove(EventHandler handler)
    {
        handlers.Remove(handler);
    }

    public void Invoke(object sender)
    {
        foreach (var handler in handlers)
        {
            handler.Invoke(sender, null);
        }
    }
}


public class FarmingEvents
{
    /// <summary>
    /// This event fires when a player finishes watering soil.
    /// </summary>
    //public static Event<WateringFinishedArgs> FinishedWateringSoil;
    public static event EventHandler<WateringFinishedArgs> FinishedWateringSoil
    {
        add => ModEntry.EventManager.FinishedWateringSoil.Add(value);
        remove => ModEntry.EventManager.FinishedWateringSoil.Remove(value);
    }
}

public class TestEvents
{
    public static event EventHandler TestEvent
    {
        add => ModEntry.EventManager.TestEvent.Add(value);
        remove => ModEntry.EventManager.TestEvent.Remove(value);
    }
}
