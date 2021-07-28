/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/ImprovedTracker
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace ImprovedTracker
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		private const int COCONUT_TREE = 9;
		private ImprovedTrackerConfig Config;

		/*********
		** Public methods
		*********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			Config = Helper.ReadConfig<ImprovedTrackerConfig>();
			helper.Events.Display.RenderedHud += this.OnRenderedHud;
		}


		/*********
		** Private methods
		*********/
		/// <summary>Raised after the HUD is rendered.  Renders additional tracking arrows if the player has the Tra-cker skill.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="args">The event data.</param>
		private void OnRenderedHud(object sender, RenderedHudEventArgs args)
		{
			// ignore if player hasn't loaded a save yet
			if (!Context.IsWorldReady)
				return;

			// If the player doesn't have the tracker skill, bail.
			if (!Game1.player.professions.Contains(Farmer.tracker))
			{
				return;
			}

			// Make a list of trackables and their color.
			List<KeyValuePair<Vector2, Color>> trackables = new List<KeyValuePair<Vector2, Color>>();

			// Track forageables underground...
			if (Config.Underground)
			{
				if (!Game1.currentLocation.IsOutdoors)
				{
					foreach (KeyValuePair<Vector2, StardewValley.Object> obj in Game1.currentLocation.objects.Pairs)
					{
						if (obj.Value.IsSpawnedObject)
						{
							trackables.Add(new KeyValuePair<Vector2, Color>(obj.Key * 64f + new Vector2(32f, 32f), Color.Yellow));
						}
					}
				}
			}

			// Track "Fish splash point" in blue...
			if (Config.FishingSpots)
			{
				if (Game1.currentLocation.fishSplashPoint.Value != null && !Game1.currentLocation.fishSplashPoint.Value.Equals(Point.Zero))
				{
					trackables.Add(new KeyValuePair<Vector2, Color>(new Vector2(Game1.currentLocation.fishSplashPoint.X, Game1.currentLocation.fishSplashPoint.Y) * 64f + new Vector2(32f, 32f), Color.Cyan));
				}
			}

			// Track berry bushes in purple...
			if (Config.Berries)
			{
				foreach (TerrainFeature feature in Game1.currentLocation.largeTerrainFeatures)
				{
					switch (feature)
					{
						case Bush bush:
							// If this bush has something to shake off and it's not a tea bush nor a golden walnut bush...
							if (bush.tileSheetOffset.Value == 1 && bush.size.Value != Bush.greenTeaBush && bush.size.Value != Bush.walnutBush)
							{
								// Add it for tracking.
								trackables.Add(new KeyValuePair<Vector2, Color>(bush.tilePosition.Value * 64f + new Vector2(64f, 32f), Color.DarkViolet));
							}
							break;
					}
				}
			}

			// Also track coconut trees in purple...
			if (Config.CoconutTrees)
			{
				foreach (TerrainFeature feature in Game1.currentLocation.terrainFeatures.Values)
				{
					switch (feature)
					{
						case Tree tree:
							// If this is a coconut tree with a visible seed...
							if (tree.treeType.Value == Tree.palmTree2 && tree.hasSeed.Value && !tree.stump.Value && tree.growthStage.Value >= 5)
							{
								// Add it for tracking.
								trackables.Add(new KeyValuePair<Vector2, Color>(new Vector2(tree.currentTileLocation.X, tree.currentTileLocation.Y) * 64f + new Vector2(32f, 32f), Color.DarkViolet));
							}
							break;
					}
				}
			}

			// Iterate through the trackables and draw them if needed.
			foreach (KeyValuePair<Vector2, Color> obj in trackables)
			{
				if (!Utility.isOnScreen(obj.Key, 64))
				{
					Microsoft.Xna.Framework.Rectangle bounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
					Vector2 onScreenPosition2 = default(Vector2);
					float rotation2 = 0f;
					if (obj.Key.X > (float)(Game1.viewport.MaxCorner.X - 64))
					{
						onScreenPosition2.X = bounds.Right - 8;
						rotation2 = (float)Math.PI / 2f;
					}
					else if (obj.Key.X < (float)Game1.viewport.X)
					{
						onScreenPosition2.X = 8f;
						rotation2 = -(float)Math.PI / 2f;
					}
					else
					{
						onScreenPosition2.X = obj.Key.X - (float)Game1.viewport.X;
					}
					if (obj.Key.Y > (float)(Game1.viewport.MaxCorner.Y - 64))
					{
						onScreenPosition2.Y = bounds.Bottom - 8;
						rotation2 = (float)Math.PI;
					}
					else if (obj.Key.Y < (float)Game1.viewport.Y)
					{
						onScreenPosition2.Y = 8f;
					}
					else
					{
						onScreenPosition2.Y = obj.Key.Y - (float)Game1.viewport.Y;
					}
					if (onScreenPosition2.X == 8f && onScreenPosition2.Y == 8f)
					{
						rotation2 += (float)Math.PI / 4f;
					}
					if (onScreenPosition2.X == 8f && onScreenPosition2.Y == (float)(bounds.Bottom - 8))
					{
						rotation2 += (float)Math.PI / 4f;
					}
					if (onScreenPosition2.X == (float)(bounds.Right - 8) && onScreenPosition2.Y == 8f)
					{
						rotation2 -= (float)Math.PI / 4f;
					}
					if (onScreenPosition2.X == (float)(bounds.Right - 8) && onScreenPosition2.Y == (float)(bounds.Bottom - 8))
					{
						rotation2 -= (float)Math.PI / 4f;
					}
					
					// Get the source rect of the arrow graphic.
					Microsoft.Xna.Framework.Rectangle srcRect = new Microsoft.Xna.Framework.Rectangle(149, 458, 5, 4);
					float renderScale = 4f;
					Vector2 safePos = Utility.makeSafe(renderSize: new Vector2((float)srcRect.Width * renderScale, (float)srcRect.Height * renderScale), renderPos: onScreenPosition2);
					args.SpriteBatch.Draw(Game1.mouseCursors, safePos, srcRect, obj.Value, rotation2, new Vector2(2f, 2f), renderScale, SpriteEffects.FlipVertically, 1f);
				}
			}
		}
	}
}