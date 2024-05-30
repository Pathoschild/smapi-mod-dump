/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Diagnostics;

using Leclair.Stardew.CloudySkies.Layers;
using Leclair.Stardew.CloudySkies.Models;

using Microsoft.Xna.Framework;

using StardewValley;

namespace Leclair.Stardew.CloudySkies;

public partial class ModEntry {

	/// <summary>
	/// Update all of our layers when the viewport moves.
	/// </summary>
	/// <returns>Whether or not we handled updating the weather.</returns>
	public bool MoveWithViewport() {
		// Before we get too far, see if we can find a custom weather that
		// we support at this location.
		var weather = Game1.currentLocation?.GetWeather();
		WeatherData? data = GetCachedWeatherData(weather?.Weather);

		// If we don't have weather data for this weather, return false
		// so the base game can handle it.
		if (data is null)
			return false;

		int offsetX = Game1.viewport.X - (int) Game1.previousViewportPosition.X;
		int offsetY = Game1.viewport.Y - (int) Game1.previousViewportPosition.Y;

		// Quick escape if nothing will change.
		if (offsetX == 0 && offsetY == 0)
			return true;

		Stopwatch? timer = Config.ShowDebugTiming ? Stopwatch.StartNew() : null;

		// It is our weather! We do it live.
		var layers = CachedLayers.Value?.Layers;
		if (layers is not null)
			foreach (var layer in layers)
				layer.MoveWithViewport(offsetX, offsetY);

		if (timer is not null) {
			timer.Stop();
			UpdateTiming.Value += timer.Elapsed.TotalMilliseconds;
		}

		return true;
	}

	/// <summary>
	/// Update all of our layers when the player moves.
	/// </summary>
	/// <returns>Whether or not we handled updating the weather.</returns>
	public bool MoveWithPlayer(int direction, float speed) {

		// Before we get too far, see if we can find a custom weather that
		// we support at this location.
		var weather = Game1.currentLocation?.GetWeather();
		WeatherData? data = GetCachedWeatherData(weather?.Weather);

		// If we don't have weather data for this weather, return false
		// so the base game can handle it.
		if (data is null)
			return false;

		int offsetX;
		int offsetY;

		switch (direction) {
			case 0: // Down
				offsetX = 0;
				offsetY = (int) speed;
				break;
			case 1: // Left
				offsetX = -(int) speed;
				offsetY = 0;
				break;
			case 2: // Up
				offsetX = 0;
				offsetY = -(int) speed;
				break;
			case 3:
				offsetX = (int) speed;
				offsetY = 0;
				break;
			default:
				return false;
		}

		// Quick escape if nothing will change.
		if (offsetX == 0 && offsetY == 0)
			return true;

		Stopwatch? timer = Config.ShowDebugTiming ? Stopwatch.StartNew() : null;

		// Update our layers.
		var layers = CachedLayers.Value?.Layers;
		if (layers is not null)
			foreach (var layer in layers)
				layer.MoveWithViewport(offsetX, offsetY);

		if (timer is not null) {
			timer.Stop();
			UpdateTiming.Value += timer.Elapsed.TotalMilliseconds;
		}

		return true;
	}


	/// <summary>
	/// Check to see if we need to update the weather for the current game
	/// instance and, if so, then update it.
	/// </summary>
	/// <param name="time">The current game time</param>
	/// <returns>Whether or not we handled updating the weather.</returns>
	public bool UpdateWeather(GameTime time) {
		// Before we get too far, see if we can find a custom weather that
		// we support at this location.
		var weather = Game1.currentLocation?.GetWeather();
		WeatherData? data = GetCachedWeatherData(weather?.Weather);

		// If we don't have weather data for this weather, return false
		// so the base game can handle it.
		if (data is null)
			return false;

		Stopwatch? timer = Config.ShowDebugTiming ? Stopwatch.StartNew() : null;

		DebrisLayer.HasUpdatedThisFrame = false;

		// It is our weather! We do it live.
		var layers = GetCachedWeatherLayers();
		if (layers is not null)
			foreach (var layer in layers)
				layer.Update(time);

		if (timer is not null) {
			timer.Stop();
			UpdateTiming.Value += timer.Elapsed.TotalMilliseconds;
		}

		return true;
	}

}
