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
using System.Reflection;

namespace Leclair.Stardew.Common.Events {
	public struct RegisteredEvent : IDisposable {

		public object EventHost;
		public EventInfo Event;
		public Delegate Delegate;

		public RegisteredEvent(object eventHost, EventInfo @event, Delegate @delegate) {
			EventHost = eventHost;
			Event = @event;
			Delegate = @delegate;
		}

		public void Dispose() {
			Event.RemoveEventHandler(EventHost, Delegate);
		}
	}
}
