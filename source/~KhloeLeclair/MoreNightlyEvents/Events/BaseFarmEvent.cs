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
using System.Diagnostics.CodeAnalysis;

using Leclair.Stardew.MoreNightlyEvents.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Netcode;

using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Delegates;
using StardewValley.Events;
using StardewValley.Network;
using StardewValley.Triggers;

namespace Leclair.Stardew.MoreNightlyEvents.Events;

public interface IInterruptable {

	void InterruptEvent();

}

public static class FarmEventInterrupter {

	internal readonly static PerScreen<IInterruptable?> ActiveEvent = new(() => null);

	public static void Interrupt() {
		foreach (var value in ActiveEvent.GetActiveValues())
			value.Value?.InterruptEvent();
	}

}


public abstract class BaseFarmEvent<T> : FarmEvent, IInterruptable, INetObject<NetFields>, IDisposable where T : BaseEventData {

	protected readonly NetString key = new NetString();
	protected T? Data;

	protected GameLocation? preEventLocation;
	protected LocationWeather? preEventWeather;

	private bool disposedValue;

	/// <summary>
	/// This event can be triggered to stop event execution immediately.
	/// </summary>
	public abstract void InterruptEvent();


	/// <summary>The multiplayer-synchronized fields for this event.</summary>
	public NetFields NetFields { get; private set; }

	public BaseFarmEvent() {
		InitNetFields();
		FarmEventInterrupter.ActiveEvent.Value = this;
	}

	public BaseFarmEvent(string key, T? data = null) : this() {
		this.key.Value = key;
		Data = data;
	}

	[MemberNotNull(nameof(NetFields))]
	public virtual void InitNetFields() {
		NetFields = new NetFields(GetType().Name)
			.SetOwner(this)
			.AddField(key);
	}

	[MemberNotNullWhen(true, nameof(Data))]
	protected bool LoadData() {
		if (Data is null && ModEntry.Instance.TryGetEvent<T>(key.Value, out var val))
			Data = val;

		return Data is not null;
	}

	protected GameLocation? GetLocation() {
		if (!LoadData())
			return null;

		string map = Data.TargetMap ?? "Farm";
		GameLocation? result = Game1.getLocationFromName(map);

		if (result is null)
			ModEntry.Instance.Log($"Unable to get map for event \"{key.Value}\": {map}", StardewModdingAPI.LogLevel.Error);

		return result;
	}

	protected void PerformSideEffects(GameLocation location, Farmer? who = null, Item? targetItem = null) {

		if (Data?.SideEffects is null || Data.SideEffects.Count == 0)
			return;

		var ctx = new GameStateQueryContext(location, who, targetItem, null, Game1.random);

		foreach(var effect in Data.SideEffects) {
			if (effect.Actions is null || effect.Actions.Count == 0)
				continue;

			if (effect.HostOnly && !Game1.IsMasterGame)
				continue;

			if (!string.IsNullOrEmpty(effect.Condition) && !GameStateQuery.CheckConditions(effect.Condition, ctx))
				continue;

			foreach (string action in effect.Actions)
				if (!TriggerActionManager.TryRunAction(action, out string? error, out Exception? ex))
					ModEntry.Instance.Log($"Error running action '{action}' for event: {error}", StardewModdingAPI.LogLevel.Error, ex);
		}
	}

	[return: NotNullIfNotNull(nameof(input))]
	protected string? Translate(string? input, Farmer? who = null, Random? rnd = null) {
		return ModEntry.Instance.TokenizeFromEvent(key.Value, input, who, rnd);
	}

	protected virtual LocationWeather? GetOverrideWeather() {
		if (string.IsNullOrEmpty(Data?.OverrideWeather))
			return null;

		// Make a new location weather + context so we can set all the right flags.
		var weather = new LocationWeather() { WeatherForTomorrow = Data.OverrideWeather };
		var ctx = new StardewValley.GameData.LocationContexts.LocationContextData() {
			WeatherConditions = []
		};

		weather.UpdateDailyWeather(null, ctx, Game1.random);
		return weather;
	}

	protected virtual bool EnterLocation() {
		// If we can't get the location for this event, then we need to
		// quit now while we're ahead.
		if (GetLocation() is not GameLocation loc)
			return false;

		// Store the current location, to be restored later.
		preEventLocation = Game1.currentLocation;

		// Flag that we are in an event, so no event gets selected when we
		// change maps and resetForPlayerEntry
		Game1.eventUp = true;

		// Do we have updated weather for the location? If so, we want
		// to set it before changing the location. That will ensure the
		// weather gets applied.
		if (GetOverrideWeather() is LocationWeather weather) {
			preEventWeather = new LocationWeather();
			var actualWeather = loc.GetWeather();
			preEventWeather.CopyFrom(actualWeather);
			actualWeather.CopyFrom(weather);
		}

		// Change the location to the location we want.
		Game1.currentLocation = loc;
		Game1.currentLocation.resetForPlayerEntry();

		// Now unset that so it doesn't mess with anything else.
		Game1.eventUp = false;

		// Update the map, and make sure its tile sheets are loaded
		// since this can be broken sometimes. Somehow.
		Game1.currentLocation.updateMap();
		Game1.currentLocation.Map.LoadTileSheets(Game1.mapDisplayDevice);

		return true;
	}


	protected virtual void LeaveLocation() {
		var oldLocation = Game1.currentLocation;

		oldLocation.cleanupBeforePlayerExit();
		oldLocation.TemporarySprites.Clear();

		// If we have an old location, move back to it.
		if (preEventLocation != null) {
			// Flag that we are in an event, so no event gets selected when we
			// change maps and resetForPlayerEntry
			Game1.eventUp = true;

			Game1.currentLocation = preEventLocation;
			Game1.currentLocation.resetForPlayerEntry();

			// Now unset that so it doesn't mess with anything else.
			Game1.eventUp = false;

			// Update the map, and make sure its tile sheets are loaded
			// since this can be broken sometimes. Somehow.
			Game1.currentLocation.updateMap();
			Game1.currentLocation.Map.LoadTileSheets(Game1.mapDisplayDevice);

			preEventLocation = null;
		}

		// Restore the old location's old weather.
		if (preEventWeather != null) {
			oldLocation.GetWeather().CopyFrom(preEventWeather);
			preEventWeather = null;
		}
	}


	/// <inheritdoc />
	public virtual bool setUp() {
		return true;
	}

	/// <inheritdoc />
	public virtual bool tickUpdate(GameTime time) {
		return true;
	}

	/// <inheritdoc />
	public virtual void draw(SpriteBatch b) {

	}

	/// <inheritdoc />
	public virtual void drawAboveEverything(SpriteBatch b) {

	}

	/// <inheritdoc />
	public virtual void makeChangesToLocation() {

	}

	protected virtual void Dispose(bool disposing) {
		if (!disposedValue) {
			if (FarmEventInterrupter.ActiveEvent.Value == this)
				FarmEventInterrupter.ActiveEvent.Value = null;

			disposedValue = true;
		}
	}

	public void Dispose() {
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
