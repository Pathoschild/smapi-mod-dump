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

using Leclair.Stardew.CloudySkies.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.TokenizableStrings;

namespace Leclair.Stardew.CloudySkies;

public partial class ModEntry {

	public readonly BlendState LightingBlend = new() {
		ColorBlendFunction = BlendFunction.ReverseSubtract,
		ColorDestinationBlend = Blend.One,
		ColorSourceBlend = Blend.SourceColor
	};

	public string GetWeatherName() {
		// Be lazy about getting the current weather, since update / drawWeather
		// will update it before we get there.
		WeatherData? data = CachedWeather.Value;

		if (data is not null)
			return data.DisplayName is null ? data.Id : TokenParser.ParseText(data.DisplayName);

		return CachedWeatherName.Value switch {
			"Sun" => I18n.Weather_Sunny(),
			"Rain" => I18n.Weather_Rain(),
			"Wind" => I18n.Weather_Debris(),
			"Storm" => I18n.Weather_Lightning(),
			"Festival" => I18n.Weather_Festival(),
			"Snow" => I18n.Weather_Snow(),
			"GreenRain" => I18n.Weather_Green(),
			_ => I18n.Weather_Sunny()
		};
	}

	public bool DrawWeatherIcon(SpriteBatch batch, Vector2 position, float scale = 4f, float layerDepth = 0.9f) {
		// Be lazy about getting the current weather, since update / drawWeather
		// will update it before we get there.
		WeatherData? data = CachedWeather.Value;

		// If we don't have weather data for this weather, return false
		// so the base game can handle it.
		if (data is null)
			return false;

		// Time to draw our icon then.
		if (data.IconTexture is null)
			return false;

		var tex = Game1.content.Load<Texture2D>(data.IconTexture);

		batch.Draw(
			tex,
			position,
			new Rectangle(
				data.IconSource.X,
				data.IconSource.Y,
				12,
				8
			),
			Color.White,
			0f,
			Vector2.Zero,
			scale,
			SpriteEffects.None,
			layerDepth
		);

		return true;
	}

	/// <summary>
	/// Check to see if we need to draw the weather for the provided game
	/// instance and, if so, then draw it.
	/// </summary>
	/// <param name="game">The game instance to draw for</param>
	/// <param name="time">The current game time</param>
	/// <param name="targetScreen">The screen to draw to.</param>
	/// <returns>Whether or not we handled drawing.</returns>
	public bool DrawWeather(Game1 game, GameTime time, RenderTarget2D targetScreen) {
		// Before we get too far, see if we can find a custom weather that
		// we support at this location.
		var weather = Game1.currentLocation?.GetWeather();
		WeatherData? data = GetCachedWeatherData(weather?.Weather);

		// If we don't have weather data for this weather, return false
		// so the base game can handle it.
		if (data is null)
			return false;

		Stopwatch? timer = Config.ShowDebugTiming ? Stopwatch.StartNew() : null;

		// Get the ModHooks. We need to be able to send out events for
		// compatibility reasons.
		var hooks = HookDelegate();

		bool old_lighting = false;

		// Start rendering with a Begin
		Game1.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp);

		// Now, we technically need to run the drawing hook.
		if (hooks.OnRendering(StardewValley.Mods.RenderSteps.World_Weather, Game1.spriteBatch, time, targetScreen) && Game1.currentLocation!.IsOutdoors) {

			// Step 2. Draw the rest of the owl.
			var layers = GetCachedWeatherLayers();
			if (layers is not null)
				foreach (var layer in layers) {
					bool lighting = layer.DrawType == LayerDrawType.Lighting;
					if (lighting != old_lighting) {
						old_lighting = lighting;
						Game1.spriteBatch.End();
						Game1.spriteBatch.Begin(
							SpriteSortMode.Texture,
							lighting ? LightingBlend : BlendState.AlphaBlend,
							SamplerState.PointClamp
						);
					}

					layer.Draw(Game1.spriteBatch, time, targetScreen);
				}
		}

		// Switch back for the final hook if we're in lighting mode.
		if (old_lighting) {
			Game1.spriteBatch.End();
			Game1.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp);
		}

		// End it now that we're done.
		hooks.OnRendered(StardewValley.Mods.RenderSteps.World_Weather, Game1.spriteBatch, time, targetScreen);
		Game1.spriteBatch.End();

		if (timer is not null) {
			timer.Stop();
			DrawTiming.Value += timer.Elapsed.TotalMilliseconds;
		}

		// And return true so the default weather doesn't draw.
		return true;
	}

}
