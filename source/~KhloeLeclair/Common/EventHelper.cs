/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Leclair.Stardew.Common.Events;

using Microsoft.Build.Utilities;

using StardewModdingAPI;

namespace Leclair.Stardew.Common;

public static class EventHelper {

	private readonly static BindingFlags EVENT_FLAGS = BindingFlags.Public | BindingFlags.Instance;
	private readonly static BindingFlags METHOD_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

	private static readonly Dictionary<object, Dictionary<Type, Tuple<object, EventInfo>?>> CachedEvents = new();

	public static Dictionary<Type, Tuple<object, EventInfo>?> GetTypes(object obj) {
		lock ((CachedEvents as ICollection).SyncRoot) {
			if (CachedEvents.ContainsKey(obj))
				return CachedEvents[obj];

			Dictionary<Type, Tuple<object, EventInfo>?> typeToEvent = new();

			ScanObjectTypes(obj, typeToEvent, 2);

			CachedEvents.Add(obj, typeToEvent);
			return typeToEvent;
		}
	}

	private static void ScanObjectTypes(object obj, Dictionary<Type, Tuple<object, EventInfo>?> typeToEvent, int recurse = 0) {
		// Events
		foreach (EventInfo evt in obj.GetType().GetEvents(EVENT_FLAGS)) {
			MethodInfo? method = evt.EventHandlerType?.GetMethod("Invoke");
			if (method == null)
				continue;

			ParameterInfo[] parms = method.GetParameters();
			if (parms.Length != 2)
				continue;

			Type type = parms[1].ParameterType;
			// In the event of duplicate events, set a null so that it won't
			// be automatically assigned. We do this to avoid ambiguous
			// subscriptions.
			if (!typeToEvent.ContainsKey(type))
				typeToEvent[type] = new Tuple<object, EventInfo>(obj, evt);
			else
				typeToEvent[type] = null;
		}

		// Recursion
		if (recurse > 0) {
			foreach (PropertyInfo prop in obj.GetType().GetProperties(EVENT_FLAGS)) {
				object? pobj = prop.GetValue(obj);
				if (pobj != null)
					ScanObjectTypes(pobj, typeToEvent, recurse - 1);
			}
		}
	}

	delegate void ConsoleCommandDelegate(string name, string[] args);

	public static void RegisterConsoleCommands(object provider, ICommandHelper helper, Action<string, LogLevel>? logger) {
		Type provtype = provider.GetType();

		foreach(MethodInfo method in provtype.GetMethods(METHOD_FLAGS)) {
			Attribute? attr = method.GetCustomAttribute(typeof(ConsoleCommand));
			if (attr is not ConsoleCommand cmd)
				continue;

			ParameterInfo[] parms = method.GetParameters();
			if (parms.Length != 2)
				continue;

			if (parms[0].ParameterType != typeof(string) || parms[1].ParameterType != typeof(string[]))
				continue;

			string name = string.IsNullOrWhiteSpace(cmd.Name) ? method.Name : cmd.Name;
			string desc = string.IsNullOrWhiteSpace(cmd.Description) ? string.Empty : cmd.Description;

			try {
				ConsoleCommandDelegate del = method.CreateDelegate<ConsoleCommandDelegate>(provider);
				helper.Add(name, desc, new Action<string, string[]>(del));
			} catch (Exception ex) {
				logger?.Invoke($"Failed to register console command {name}: {ex}", LogLevel.Error);
			}
		}
	}


	public static Dictionary<MethodInfo, RegisteredEvent> RegisterEvents(object subscriber, object eventBus, Dictionary<MethodInfo, RegisteredEvent>? existing, Action<string, LogLevel>? logger) {
		Dictionary<Type, Tuple<object, EventInfo>?> typeToEvent = GetTypes(eventBus);
		Dictionary<MethodInfo, RegisteredEvent> Events = existing ?? new();

		lock ((Events as ICollection).SyncRoot) {
			Type subtype = subscriber.GetType();

			foreach (MethodInfo method in subtype.GetMethods(METHOD_FLAGS)) {
				Attribute? attr = method.GetCustomAttribute(typeof(Subscriber));
				if (attr is not Subscriber || Events.ContainsKey(method))
					continue;

				ParameterInfo[] parms = method.GetParameters();
				if (parms.Length != 2)
					continue;

				Type type = parms[1].ParameterType;
				Tuple<object, EventInfo>? pair = null;
				lock ((typeToEvent as ICollection).SyncRoot) {
					typeToEvent.TryGetValue(type, out pair);
				}

				if (pair == null)
					continue;

				EventInfo evt = pair.Item2;
				if (evt.EventHandlerType == null)
					continue;

				logger?.Invoke($"Registering event {subtype.Name}.{method.Name} for event {evt.Name}", LogLevel.Trace);

				Delegate del;
				try {
					del = method.CreateDelegate(evt.EventHandlerType, subscriber);
					evt.AddEventHandler(pair.Item1, del);
				} catch (Exception ex) {
					logger?.Invoke($"Failed to register event {subtype.Name}.{method.Name} for event {evt.Name}", LogLevel.Error);
					logger?.Invoke(ex.ToString(), LogLevel.Error);
					continue;
				}

				Events.Add(method, new RegisteredEvent(pair.Item1, evt, del));
			}
		}

		return Events;
	}


	public static void UnregisterEvents(Dictionary<MethodInfo, RegisteredEvent>? events) {
		if (events == null)
			return;

		lock ((events as ICollection).SyncRoot) {
			foreach (RegisteredEvent evt in events.Values)
				evt.Dispose();

			events.Clear();
		}
	}
}
