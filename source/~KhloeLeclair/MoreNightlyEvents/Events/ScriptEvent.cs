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

using Leclair.Stardew.MoreNightlyEvents.Models;

using Microsoft.Xna.Framework;

using StardewValley;

using SDVEvent = StardewValley.Event;

namespace Leclair.Stardew.MoreNightlyEvents.Events;

public class ScriptEvent : BaseFarmEvent<ScriptEventData> {

	internal SDVEvent? Event;
	internal bool Running;

	public ScriptEvent() : base() { }

	public ScriptEvent(string key, ScriptEventData? data = null) : base(key, data) {

	}

	#region FarmEvent

	public override bool setUp() {
		if (! LoadData() || string.IsNullOrEmpty(Data.Script))
			return true;

		// Try to load our new map. If we can't, then quit
		// now because nothing will work.
		if (!EnterLocation())
			return true;

		var loc = Game1.currentLocation;

		// Start doing a fade.
		Game1.fadeClear();
		Game1.nonWarpFade = true;

		// Set up the time of day, weather, light, etc.
		Game1.timeOfDay = Data.TimeOfDay ?? 2400;
		Game1.ambientLight = Data.AmbientLight ?? new Color(200, 190, 40);

		// If we have a target point, move the viewport
		if (Data.TargetPoint.HasValue) {
			var targetTile = Data.TargetPoint.Value;
			if (!loc.IsOutdoors) {
				Game1.viewport.X = targetTile.X * 64 - Game1.viewport.Width / 2;
				Game1.viewport.Y = targetTile.Y * 64 - Game1.viewport.Height / 2;
			} else {
				Game1.viewport.X = Math.Max(0, Math.Min(loc.map.DisplayWidth - Game1.viewport.Width, targetTile.X * 64 - Game1.viewport.Width / 2));
				Game1.viewport.Y = Math.Max(0, Math.Min(loc.map.DisplayHeight - Game1.viewport.Height, targetTile.Y * 64 - Game1.viewport.Height / 2));
			}

			Game1.previousViewportPosition = new Vector2(Game1.viewport.X, Game1.viewport.Y);
		}

		// Create a fake farmer for the event.
		Farmer fake = Game1.player.CreateFakeEventFarmer();
		fake.completelyStopAnimatingOrDoingAction();
		fake.hidden.Value = false;
		fake.currentLocation = Game1.currentLocation;

		// Create our new Event instance using the script
		// from this event and our fake farmer.
		Event = new SDVEvent(
			eventString: Translate(Data.Script, Game1.player),
			farmerActor: fake
		);

		// Add a finished delegate that will fade the
		// screen and change the music track to none.
		Event.onEventFinished += afterFade;

		// And start our event.
		Game1.currentLocation.startEvent(Event);

		// The Running flag is important so we know the event
		// isn't over yet. It's changed after the fade.
		Running = true;
		return false;
	}

	public void afterFade() {
		Running = false;
		cleanUp();
	}

	public void cleanUp() {
		// Clean up after the event in every way we know how.
		Event?.cleanup();
		if (Game1.currentLocation.currentEvent == Event)
			Game1.currentLocation.currentEvent = null;

		Game1.changeMusicTrack("none");

		Game1.eventOver = true;
		Game1.eventUp = false;
		//Game1.eventFinished();

		// Perform any side effects.
		PerformSideEffects(Game1.currentLocation, Game1.player, null);

		// Go back to the previous location.
		LeaveLocation();

		// Make sure the player isn't in a weird state.
		Game1.player.ignoreCollisions = false;
		Game1.player.CanMove = true;
		Game1.freezeControls = false;
		Game1.displayFarmer = true;
		Game1.player.forceCanMove();
	}

	public override void InterruptEvent() {
		Running = false;
	}

	public override bool tickUpdate(GameTime time) {
		// If the event isn't running anymore, then stop
		// and do our cleanup.
		if (!Running || Event is null || Event.skipped) {
			cleanUp();
			return true;
		}

		// There is no farm event in Ba Sing Se.
		var evt = Game1.farmEvent;
		Game1.farmEvent = null;

		// We need to manually update things, since farm events don't
		// do the normal update loop. This is the same set of
		// update instructions as the vanilla fairy event, so I
		// am reasonably sure things are safe.
		Game1.UpdateGameClock(time);

		Game1.currentLocation.UpdateWhenCurrentLocation(time);
		Game1.currentLocation.updateEvenIfFarmerIsntHere(time);

		Game1.UpdateOther(time);

		// Okay maybe there's a farm event.
		Game1.farmEvent = evt;

		return false;
	}

	#endregion
}
