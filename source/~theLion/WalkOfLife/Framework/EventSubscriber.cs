/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.Events;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework
{
	/// <summary>Manages dynamic subscribing and unsubscribing of events for modded professions.</summary>
	internal class EventSubscriber
	{
		private Action<string, LogLevel> Log { get; }

		private static readonly Dictionary<string, List<BaseEvent>> EventsByProfession = new()
		{
			{"Conservationist", new() {new ConservationistDayEndingEvent()}},
			{"Poacher", new() {new PoacherWarpedEvent()}},
			{"Piper", new() {new PiperWarpedEvent()}},
			{
				"Prospector",
				new()
				{
					new ProspectorHuntDayStartedEvent(),
					new ProspectorWarpedEvent(),
					new TrackerButtonsChangedEvent()
				}
			},
			{
				"Scavenger",
				new() {new ScavengerHuntDayStartedEvent(), new ScavengerWarpedEvent(), new TrackerButtonsChangedEvent()}
			},
			{"Spelunker", new() {new SpelunkerWarpedEvent()}}
		};

		private readonly List<BaseEvent> _subscribed = new();

		/// <summary>Construct an instance.</summary>
		internal EventSubscriber(Action<string, LogLevel> log)
		{
			Log = log;

			// hook static events
			SubscribeStaticEvents();
		}

		internal IEnumerable<string> SubscribedEvents => _subscribed.Select(e => e.GetType().Name);

		/// <summary>Subscribe new events to the event listener.</summary>
		/// <param name="events">Events to be subscribed.</param>
		internal void Subscribe(params BaseEvent[] events)
		{
			foreach (var e in events)
				if (_subscribed.ContainsType(e.GetType()))
				{
					Log($"[EventSubscriber]: Farmer already subscribed to {e.GetType().Name}.",
						LogLevel.Trace);
				}
				else
				{
					e.Hook();
					_subscribed.Add(e);
					Log($"[EventSubscriber]: Subscribed to {e.GetType().Name}.", LogLevel.Trace);
				}
		}

		/// <summary>Unsubscribe events from the event listener.</summary>
		/// <param name="eventTypes">The event types to be unsubscribed.</param>
		internal void Unsubscribe(params Type[] eventTypes)
		{
			foreach (var type in eventTypes)
				if (_subscribed.RemoveType(type, out var removed))
				{
					removed.Unhook();
					Log($"[EventSubscriber]: Unsubscribed from {type.Name}.", LogLevel.Trace);
				}
				else
				{
					Log($"[EventSubscriber]: Farmer not subscribed to {type.Name}.", LogLevel.Trace);
				}
		}

		/// <summary>Subscribe the event listener to events required for basic mod function.</summary>
		internal void SubscribeStaticEvents()
		{
			Log("[EventSubscriber]: Subscribing static events...", LogLevel.Trace);
			Subscribe(new StaticGameLaunchedEvent(), new StaticSaveLoadedEvent(), new StaticReturnedToTitleEvent(),
				new StaticLevelChangedEvent(), new StaticSuperModeIndexChangedEvent());

			if (!ModEntry.ModHelper.ModRegistry.IsLoaded("alphablackwolf.skillPrestige") &&
			    !ModEntry.ModHelper.ModRegistry.IsLoaded("cantorsdust.AllProfessions"))
				return;

			Log(
				"[EventSubscriber]: Skill Prestige or All Professions mod detected. Subscribing additional fail-safe event.",
				LogLevel.Trace);
			Subscribe(new StaticDayEndingEvent());
		}

		/// <summary>Subscribe the event listener to all events required by the local player's current professions.</summary>
		internal void SubscribeEventsForLocalPlayer()
		{
			Log($"[EventSubscriber]: Subscribing dynamic events for farmer {Game1.player.Name}...",
				LogLevel.Trace);
			foreach (var professionIndex in Game1.player.professions)
				try
				{
					SubscribeEventsForProfession(Util.Professions.NameOf(professionIndex));
				}
				catch (IndexOutOfRangeException)
				{
					Log($"[EventSubscriber]: Unexpected profession index {professionIndex} will be ignored.",
						LogLevel.Trace);
				}

			Log("[EventSubscriber]: Done subscribing player events.", LogLevel.Trace);
		}

		/// <summary>Subscribe the event listener to all events required by the local player's current professions.</summary>
		internal void UnsubscribeLocalPlayerEvents()
		{
			Log("[EventSubscriber]: Unsubscribing player dynamic events...", LogLevel.Trace);
			Unsubscribe(_subscribed.Where(s => s.Prefix() != "Static").Select(subscribed => subscribed.GetType())
				.ToArray());
			Log("[EventSubscriber]: Done unsubscribing player events.", LogLevel.Trace);
		}

		/// <summary>Subscribe the event listener to all events required by a specific profession.</summary>
		/// <param name="whichProfession">The profession index.</param>
		internal void SubscribeEventsForProfession(string whichProfession)
		{
			if (!EventsByProfession.TryGetValue(whichProfession, out var events)) return;

			Log($"[EventSubscriber]: Subscribing to {whichProfession} profession events...", LogLevel.Trace);
			foreach (var e in events) Subscribe(e);
			Log("[EventSubscriber]: Done subscribing profession events.", LogLevel.Trace);
		}

		/// <summary>Unsubscribe the event listener from all events required by a specific profession.</summary>
		/// <param name="whichProfession">The profession index.</param>
		internal void UnsubscribeProfessionEvents(string whichProfession)
		{
			if (!EventsByProfession.TryGetValue(whichProfession, out var events)) return;

			List<BaseEvent> except = new();
			if (whichProfession == "Prospector" && Game1.player.HasProfession("Scavenger") ||
			    whichProfession == "Scavenger" && Game1.player.HasProfession("Prospector"))
				except.Add(new TrackerButtonsChangedEvent());

			Log($"[EventSubscriber]: Unsubscribing from {whichProfession} profession events...",
				LogLevel.Trace);
			foreach (var e in events.Except(except)) Unsubscribe(e.GetType());
			Log("[EventSubscriber]: Done unsubscribing profession events.", LogLevel.Trace);
		}

		/// <summary>Subscribe the event listener to all events required for super mode functionality.</summary>
		internal void SubscribeSuperModeEvents()
		{
			Subscribe(
				new SuperModeButtonsChangedEvent(),
				new SuperModeCounterFilledEvent(),
				new SuperModeCounterRaisedAboveZeroEvent(),
				new SuperModeCounterReturnedToZeroEvent(),
				new SuperModeDisabledEvent(),
				new SuperModeEnabledEvent(),
				new SuperModeWarpedEvent()
			);

			if (!Game1.currentLocation.IsCombatZone() && ModEntry.SuperModeCounter <= 0) return;

			ModEntry.Subscriber.Subscribe(new SuperModeBarRenderingHudEvent());
			if (ModEntry.SuperModeCounter >= ModEntry.SuperModeCounterMax)
				ModEntry.Subscriber.Subscribe(new SuperModeBarShakeTimerUpdateTickedEvent());
		}

		/// <summary>Unsubscribe the event listener from all events related to super mode functionality.</summary>
		internal void UnsubscribeSuperModeEvents()
		{
			Unsubscribe(
				typeof(SuperModeActivationTimerUpdateTickedEvent),
				typeof(SuperModeBarFadeOutUpdateTickedEvent),
				typeof(SuperModeBarRenderingHudEvent),
				typeof(SuperModeBarShakeTimerUpdateTickedEvent),
				typeof(SuperModeBuffDisplayUpdateTickedEvent),
				typeof(SuperModeButtonsChangedEvent),
				typeof(SuperModeCounterFilledEvent),
				typeof(SuperModeCounterRaisedAboveZeroEvent),
				typeof(SuperModeCounterReturnedToZeroEvent),
				typeof(SuperModeDisabledEvent),
				typeof(SuperModeEnabledEvent),
				typeof(SuperModeWarpedEvent)
			);
		}

		/// <summary>Check if there are rogue events still subscribed and remove them.</summary>
		internal void CleanUpRogueEvents()
		{
			Log("[EventSubscriber]: Checking for rogue profession events...", LogLevel.Trace);
			foreach (var e in _subscribed
				.Where(e =>
					Util.Professions.IndexByName.Contains(e.Prefix()) && !Game1.player.HasProfession(e.Prefix()) ||
					e.Prefix() == "Tracker" && !Game1.player.HasAnyOfProfessions("Prospector", "Scavenger") ||
					e.Prefix() == "SuperMode" &&
					!Game1.player.HasAnyOfProfessions("Brute", "Poacher", "Piper", "Desperado"))
				.Reverse()) Unsubscribe(e.GetType());
			Log("[EventSubscriber]: Done unsubscribing rogue events.", LogLevel.Trace);
		}

		/// <summary>Whether the event listener is subscribed to a given event type.</summary>
		/// <param name="eventType">The event type to check.</param>
		internal bool IsSubscribed(Type eventType)
		{
			return _subscribed.ContainsType(eventType);
		}
	}
}