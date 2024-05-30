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
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Buildings;

namespace Leclair.Stardew.BCBuildings;

public class BuildingRenderer {

	public readonly ModEntry Mod;

	private readonly List<((string, string?)?, Building?, Action<Texture2D>)> renderQueue = new();

	public BuildingRenderer(ModEntry mod) {
		Mod = mod;

		Mod.Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

	}

	private void OnUpdateTicked(object? sender, StardewModdingAPI.Events.UpdateTickedEventArgs e) {
		if (renderQueue.Count > 0) {
			var item = renderQueue[0];
			renderQueue.RemoveAt(0);

			var tex = DoRender(item.Item1, item.Item2);
			item.Item3(tex);
		}
	}

	public void RenderBuilding(string id, string? skinId, Action<Texture2D> onComplete) {
		renderQueue.Add(((id, skinId), null, onComplete));
	}

	public void RenderBuilding(Building building, Action<Texture2D> onComplete) {
		renderQueue.Add((null, building, onComplete));
	}

	private Texture2D DoRender((string, string?)? inputId, Building? inputBuilding) {
		//string id, string? skinId) {
		Building building;

		if (inputBuilding != null)
			building = inputBuilding;

		else if (!inputId.HasValue)
			throw new ArgumentNullException(nameof(inputId));

		else {
			building = Building.CreateInstanceFromId(inputId.Value.Item1, Vector2.Zero);
			building.skinId.Value = inputId.Value.Item2;
		}

		Rectangle size = building.getSourceRect();
		if (building is FishPond) {
			size = new(0, 0, 80, 112);
		}

		int width = size.Width * 2;
		int height = size.Height * 2;

		var big_target = new RenderTarget2D(
			Game1.graphics.GraphicsDevice, width * 2, height * 2, false,
			SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents
		);

		var old_targets = Game1.spriteBatch.GraphicsDevice.GetRenderTargets();

		Game1.spriteBatch.GraphicsDevice.SetRenderTarget(big_target);
		Game1.spriteBatch.GraphicsDevice.Clear(Color.Transparent);

		Game1.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, Utility.ScissorEnabled);

		building.drawInMenu(Game1.spriteBatch, 0, building is FishPond ? 64 : 0);

		Game1.spriteBatch.End();

		var target = new RenderTarget2D(
			Game1.graphics.GraphicsDevice, width, height, false,
			SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents
		);

		Game1.spriteBatch.GraphicsDevice.SetRenderTarget(target);
		Game1.spriteBatch.GraphicsDevice.Clear(Color.Transparent);

		Game1.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, Utility.ScissorEnabled);

		Game1.spriteBatch.Draw(big_target, new Rectangle(0, 0, width, height), Color.White);

		Game1.spriteBatch.End();

		Game1.spriteBatch.GraphicsDevice.SetRenderTargets(old_targets);

		big_target.Dispose();
		big_target = null;

		Color[] colors = new Color[width * height];
		target.GetData(colors);

		var result = new Texture2D(Game1.spriteBatch.GraphicsDevice, width, height, false, SurfaceFormat.Color);
		result.SetData(colors);

		target.Dispose();

		return result;
	}


}
