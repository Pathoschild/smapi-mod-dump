/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Services;

using System.Reflection;
using StardewMods.FauxCore.Common.Helpers;
using StardewMods.FauxCore.Common.Interfaces;
using StardewMods.FauxCore.Common.Models;

#else
namespace StardewMods.Common.Services;

using System.Reflection;
using StardewMods.Common.Helpers;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Models;
#endif

/// <summary>Represents a base event manager service.</summary>
internal class BaseEventManager : IEventManager
{
    private static readonly ReverseComparer<int> ReverseComparer = new();

    /// <summary>Gets the subscribers.</summary>
    protected Dictionary<Type, SortedList<int, List<Delegate>>> Subscribers { get; } = new();

    /// <inheritdoc />
    public virtual void Publish<TEventArgs>(TEventArgs eventArgs)
        where TEventArgs : EventArgs
    {
        var eventType = typeof(TEventArgs);
        SortedList<int, List<Delegate>> handlersToInvoke;
        lock (this.Subscribers)
        {
            if (!this.Subscribers.TryGetValue(eventType, out var priorityHandlers))
            {
                return;
            }

            handlersToInvoke = new SortedList<int, List<Delegate>>(priorityHandlers, BaseEventManager.ReverseComparer);
        }

        foreach (var priorityGroup in handlersToInvoke.Values)
        {
            var handlers = priorityGroup.ToList();
            foreach (var @delegate in handlers)
            {
                if (@delegate is not Action<TEventArgs> handler)
                {
                    continue;
                }

                try
                {
                    handler(eventArgs);
                }
                catch (Exception ex)
                {
                    Log.Trace("Exception occured: {0}\n{1}", ex.Message, ex.StackTrace);
                }
            }
        }
    }

    /// <inheritdoc />
    public virtual void Publish<TEventType, TEventArgs>(TEventArgs eventArgs)
        where TEventArgs : EventArgs, TEventType
    {
        var eventType = typeof(TEventType);
        SortedList<int, List<Delegate>> handlersToInvoke;
        lock (this.Subscribers)
        {
            if (!this.Subscribers.TryGetValue(eventType, out var priorityHandlers))
            {
                return;
            }

            handlersToInvoke = new SortedList<int, List<Delegate>>(priorityHandlers);
        }

        foreach (var priorityGroup in handlersToInvoke.Values)
        {
            var handlers = priorityGroup.ToList();
            foreach (var @delegate in handlers)
            {
                if (@delegate is not Action<TEventArgs> handler)
                {
                    continue;
                }

                try
                {
                    handler(eventArgs);
                }
                catch (Exception ex)
                {
                    Log.Trace("Exception occured: {0}\n{1}", ex.Message, ex.StackTrace);
                }
            }
        }
    }

    /// <inheritdoc />
    public virtual void Subscribe<TEventArgs>(Action<TEventArgs> handler)
    {
        var eventType = typeof(TEventArgs);
        lock (this.Subscribers)
        {
            if (!this.Subscribers.TryGetValue(eventType, out var priorityHandlers))
            {
                priorityHandlers = new SortedList<int, List<Delegate>>();
                this.Subscribers.Add(eventType, priorityHandlers);
            }

            var methodInfo = handler.Method;
            var priorityAttribute = methodInfo.GetCustomAttribute<PriorityAttribute>();
            var priority = priorityAttribute?.Priority ?? 0;
            if (!priorityHandlers.TryGetValue(priority, out var handlers))
            {
                handlers = new List<Delegate>();
                priorityHandlers.Add(priority, handlers);
            }

            handlers.Add(handler);
        }
    }

    /// <inheritdoc />
    public virtual void Unsubscribe<TEventArgs>(Action<TEventArgs> handler)
    {
        var eventType = typeof(TEventArgs);
        lock (this.Subscribers)
        {
            if (!this.Subscribers.TryGetValue(eventType, out var priorityHandlers))
            {
                return;
            }

            var methodInfo = handler.Method;
            var priorityAttribute = methodInfo.GetCustomAttribute<PriorityAttribute>();
            var priority = priorityAttribute?.Priority ?? 0;
            if (!priorityHandlers.TryGetValue(priority, out var handlers))
            {
                return;
            }

            handlers.Remove(handler);
            if (priorityHandlers.Count != 0)
            {
                return;
            }

            this.Subscribers.Remove(eventType);
        }
    }
}