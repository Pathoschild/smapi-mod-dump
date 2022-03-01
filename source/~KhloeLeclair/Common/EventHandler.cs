/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Leclair.Stardew.Common {
	public abstract class EventSubscriber<T> : IDisposable where T : Mod {

		private static readonly Dictionary<Mod, Dictionary<Type, Tuple<object, EventInfo>>> CachedTypes = new();

		private static Dictionary<Type, Tuple<object, EventInfo>> GetTypes(Mod mod) {
			lock((CachedTypes as ICollection).SyncRoot) {
				if (CachedTypes.ContainsKey(mod))
					return CachedTypes[mod];

				IModEvents EvHelper = mod.Helper.Events;
				Dictionary<Type, Tuple<object, EventInfo>> typeToEvent = new();

				foreach(PropertyInfo prop in EvHelper.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
					foreach(EventInfo evt in prop.PropertyType.GetEvents()) {
						MethodInfo method = evt.EventHandlerType.GetMethod("Invoke");
						ParameterInfo[] parms = method.GetParameters();

						if (parms.Length != 2)
							continue;

						Type type = parms[1].ParameterType;
						if (!typeToEvent.ContainsKey(type))
							typeToEvent[type] = new Tuple<object, EventInfo>(prop.GetValue(EvHelper), evt);
					}
				}

				CachedTypes.Add(mod, typeToEvent);
				return typeToEvent;
			}
		}

		private struct RegisteredEvent {
			public Delegate Delegate;
			public object Object;
			public EventInfo Event;

			public RegisteredEvent(Delegate @delegate, object obj, EventInfo @event) {
				Delegate = @delegate;
				Object = obj;
				Event = @event;
			}
		}

		public readonly T Mod;

		private Dictionary<MethodInfo,RegisteredEvent> Events;


		public EventSubscriber(T mod, bool registerImmediate = true) {
			Mod = mod;

			if (registerImmediate)
				RegisterEvents();
		}

		public virtual void Dispose() {
			UnregisterEvents();
		}

		public void RegisterEvents() {
			Dictionary<Type, Tuple<object, EventInfo>> typeToEvent = GetTypes(Mod);

			if (Events == null)
				Events = new Dictionary<MethodInfo, RegisteredEvent>();

			lock ((Events as ICollection).SyncRoot) {
				foreach (MethodInfo method in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)) {
					System.Attribute attr = method.GetCustomAttribute(typeof(Subscriber));
					if (attr is not Subscriber || Events.ContainsKey(method))
						continue;

					ParameterInfo[] paras = method.GetParameters();
					if (paras.Length != 2)
						continue;

					Type type = paras[1].ParameterType;
					Tuple<object, EventInfo> pair = null;
					lock ((typeToEvent as ICollection).SyncRoot) {
						typeToEvent.TryGetValue(type, out pair);
					}

					if (pair == null)
						continue;

					EventInfo evt = pair.Item2;
					Mod.Monitor.Log($"Registering event {GetType().Name}.{method.Name} for event {evt.Name}.", LogLevel.Trace);

					Delegate del;
					try {
						del = method.CreateDelegate(evt.EventHandlerType, this);
						evt.AddEventHandler(pair.Item1, del);
					} catch (Exception ex) {
						Mod.Monitor.Log($"Failed to register event {GetType().Name}.{method.Name} for event {evt.Name}", LogLevel.Error);
						Mod.Monitor.Log(ex.ToString(), LogLevel.Error);
						continue;
					}

					Events.Add(method, new RegisteredEvent(del, pair.Item1, evt));
				}
			}
		}

		public void UnregisterEvents() {
			if (Events == null)
				return;

			lock ((Events as ICollection).SyncRoot) {
				foreach(RegisteredEvent evt in Events.Values) {
					evt.Event.RemoveEventHandler(evt.Object, evt.Delegate);
				}

				Events.Clear();
			}

			Events = null;
		}
		

	}
}
