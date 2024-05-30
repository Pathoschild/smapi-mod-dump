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

using StardewModdingAPI;

using StardewValley;
using StardewValley.Delegates;
using StardewValley.Triggers;

namespace Leclair.Stardew.Common;

public static class EventHelper {

	private readonly static BindingFlags EVENT_FLAGS = BindingFlags.Public | BindingFlags.Instance;
	private readonly static BindingFlags METHOD_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
	private readonly static BindingFlags STATIC_METHOD_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

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

	public static List<string> RegisterConsoleCommands(object provider, ICommandHelper helper, Action<string, LogLevel>? logger) {
		bool is_static;
		if (provider is Type provType)
			is_static = true;
		else {
			is_static = false;
			provType = provider.GetType();
		}

		List<string> result = new();

		foreach (MethodInfo method in provType.GetMethods(is_static ? STATIC_METHOD_FLAGS : METHOD_FLAGS)) {

			Action<string, string[]>? @delegate = null;

			foreach (var attr in method.GetCustomAttributes<ConsoleCommand>()) {

				if (@delegate is null)
					try {
						@delegate = new Action<string, string[]>(
							is_static
								? method.CreateDelegate<ConsoleCommandDelegate>()
								: method.CreateDelegate<ConsoleCommandDelegate>(provider)
							);

					} catch (Exception ex) {
						logger?.Invoke($"Failed to register console command. Method {method.Name} does not match console command delegate: {ex}", LogLevel.Error);
						break;
					}

				string name = string.IsNullOrWhiteSpace(attr.Name) ? method.Name : attr.Name;
				string desc = string.IsNullOrWhiteSpace(attr.Description) ? string.Empty : attr.Description;

				try {
					helper.Add(name, desc, @delegate);
					result.Add(name);
				} catch (Exception ex) {
					logger?.Invoke($"Failed to register console command '{name}': {ex}", LogLevel.Error);
				}
			}
		}

		return result;
	}


	public static List<string> RegisterTriggerActions(object provider, string? prefix, Action<string, LogLevel>? logger) {
		bool is_static;
		if (provider is Type provType)
			is_static = true;
		else {
			is_static = false;
			provType = provider.GetType();
		}

		List<string> result = new();

		foreach (MethodInfo method in provType.GetMethods(is_static ? STATIC_METHOD_FLAGS : METHOD_FLAGS)) {

			TriggerActionDelegate? @delegate = null;

			foreach (var attr in method.GetCustomAttributes<TriggerAction>()) {

				if (@delegate is null)
					try {
						@delegate = is_static
							? method.CreateDelegate<TriggerActionDelegate>()
							: method.CreateDelegate<TriggerActionDelegate>(provider);
					} catch (Exception ex) {
						logger?.Invoke($"Failed to register trigger action. Method {method.Name} does not match trigger delegate: {ex}", LogLevel.Error);
						break;
					}

				string name = string.IsNullOrWhiteSpace(attr.Name) ? method.Name : attr.Name;
				if (!string.IsNullOrEmpty(prefix) && !attr.SkipPrefix)
					name = prefix + name;

				try {
					TriggerActionManager.RegisterAction(name, @delegate);
					result.Add(name);
				} catch (Exception ex) {
					logger?.Invoke($"Failed to register trigger action '{name}': {ex}", LogLevel.Error);
				}
			}
		}

		return result;
	}

	public static List<string> RegisterGameStateQueries(object provider, string?[]? prefixes, Action<string, LogLevel>? logger) {
		bool is_static;
		if (provider is Type provType)
			is_static = true;
		else {
			is_static = false;
			provType = provider.GetType();
		}

		if (prefixes is null || prefixes.Length == 0)
			prefixes = [null];

		List<string> result = new();

		foreach (MethodInfo method in provType.GetMethods(is_static ? STATIC_METHOD_FLAGS : METHOD_FLAGS)) {
			GameStateQueryDelegate? @delegate = null;
			string? first_name = null;

			foreach (var attr in method.GetCustomAttributes<GSQCondition>()) {

				if (@delegate is null)
					try {
						@delegate = is_static
							? method.CreateDelegate<GameStateQueryDelegate>()
							: method.CreateDelegate<GameStateQueryDelegate>(provider);

					} catch (Exception ex) {
						logger?.Invoke($"Failed to register game state query condition. Method {method.Name} does not match delegate: {ex}", LogLevel.Error);
						break;
					}

				string name = string.IsNullOrWhiteSpace(attr.Name) ? method.Name : attr.Name;

				foreach (string? pre in attr.SkipPrefix ? [null] : prefixes) {
					string prename = name;
					if (!string.IsNullOrWhiteSpace(pre))
						prename = pre + name;

					try {
						if (first_name is null) {
							GameStateQuery.Register(prename, @delegate);
							result.Add(prename);
							first_name = prename;

						} else if (!GameStateQuery.Exists(prename)) {
							GameStateQuery.RegisterAlias(prename, first_name);
							result.Add(prename);
						}

					} catch (Exception ex) {
						logger?.Invoke($"Failed to register game state query condition '{prename}': {ex}", LogLevel.Error);
					}
				}
			}
		}

		return result;
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
