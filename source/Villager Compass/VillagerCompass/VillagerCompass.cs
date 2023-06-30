/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jbossjaslow/Stardew_Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;

namespace VillagerCompass {
	internal class VillagerCompass : IDisposable {
		private readonly IModHelper _helper;
		private readonly IMonitor _monitor;
		private readonly ModConfig _config;

		private readonly PerScreen<int> _pauseTicks = new(createNewState: () => 60);
		private float _rotation = 0;
		private readonly int scale = 1;
		private bool currentNPCIsNotInLocation = false;

		public NPC? npcToFind;

		// Custom indicator properties
		private readonly Texture2D defaultIndicatorTexture;
		//private readonly Texture2D? customIndicatorTexture;
		private readonly Rectangle defaultIndicatorBounds;
		//private readonly Rectangle? customIndicatorBounds;

		public VillagerCompass(IModHelper helper, IMonitor monitor, ModConfig config) {
			_helper = helper;
			_monitor = monitor;
			_config = config;

			defaultIndicatorTexture = Game1.mouseCursors;
			defaultIndicatorBounds = new(x: 324, y: 477, width: 7, height: 19);

			//try {
			//	customIndicatorTexture = _helper.ModContent.Load<Texture2D>("Customization/compassPointer.png");
			//	customIndicatorBounds = customIndicatorTexture.Bounds;
			//	_monitor.Log($"Custom indicator exists", LogLevel.Debug);
			//} catch {
			//	_monitor.Log($"Custom indicator does not exist", LogLevel.Info);
			//}

			//destinationRectangle = new(
			//	x: Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - 250,
			//	y: Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 50,
			//	width: 7 * scale,
			//	height: 19 * scale
			//);
		}
		public void ToggleMod(bool isOn) {
			//_helper.Events.Player.Warped -= OnWarped;
			//_helper.Events.GameLoop.UpdateTicked -= UpdateTicked;
			_helper.Events.Display.RenderingHud -= OnRenderingHud;

			if (isOn) {
				//_helper.Events.Player.Warped += OnWarped;
				//_helper.Events.GameLoop.UpdateTicked += UpdateTicked;
				_helper.Events.Display.RenderingHud += OnRenderingHud;
			}
		}

		private void OnRenderingHud(object? sender, RenderingHudEventArgs e) {
			//_monitor.Log($"{Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea}", LogLevel.Debug);
			if (npcToFind == null) return;

			if (Game1.CurrentEvent != null && !Game1.CurrentEvent.isFestival) return;

			Rectangle destinationRectangle = new(
				x: Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - 250,
				y: Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 80,
				width: defaultIndicatorBounds.Width * 3,
				height: defaultIndicatorBounds.Height * 3
			);

			RotateCompass(npcToFind, destinationRectangle);

			Game1.spriteBatch.Draw(
				texture: defaultIndicatorTexture,
				destinationRectangle: destinationRectangle,
				sourceRectangle: defaultIndicatorBounds,
				color: Color.White,
				rotation: _rotation,
				origin: new(0.5f + defaultIndicatorBounds.Width / 2, 0.5f + defaultIndicatorBounds.Height / 2),//new(4, 10),
				effects: SpriteEffects.None,
				layerDepth: 1f
			);

			DrawCompassPoints(destinationRectangle);
		}

		private static void DrawCompassPoints(Rectangle arrowRect) {
			Vector2 center = Utility.PointToVector2(arrowRect.Center);
			float xOff = -arrowRect.Width * 0.75f - 3;
			float yOff = -arrowRect.Height * 0.75f;

			Game1.spriteBatch.DrawString(
				spriteFont: Game1.smallFont,
				text: "N",
				position: new(x: center.X + xOff,
							  y: center.Y - arrowRect.Height + yOff),
				color: Color.White
			);

			Game1.spriteBatch.DrawString(
				spriteFont: Game1.smallFont,
				text: "S",
				position: new(x: center.X + xOff,
							  y: center.Y + arrowRect.Height + yOff),
				color: Color.White
			);

			Game1.spriteBatch.DrawString(
				spriteFont: Game1.smallFont,
				text: "E",
				position: new(x: center.X + arrowRect.Height + xOff,
							  y: center.Y + yOff),
				color: Color.White
			);

			Game1.spriteBatch.DrawString(
				spriteFont: Game1.smallFont,
				text: "W",
				position: new(x: center.X - arrowRect.Height + xOff,
							  y: center.Y + yOff),
				color: Color.White
			);

			//Game1.spriteBatch.DrawString(
			//	spriteFont: Game1.smallFont,
			//	text: "N",
			//	position: new(x: arrowRect.Left - arrowRect.Width / 2,
			//				  y: arrowRect.Top - arrowRect.Height),
			//	color: Color.White
			//);

			//Game1.spriteBatch.DrawString(
			//	spriteFont: Game1.smallFont,
			//	text: "S",
			//	position: new(x: arrowRect.Left - arrowRect.Width / 2,
			//				  y: arrowRect.Top + arrowRect.Height / 2),
			//	color: Color.White
			//);

			//Game1.spriteBatch.DrawString(
			//	spriteFont: Game1.smallFont,
			//	text: "E",
			//	position: new(x: arrowRect.X + arrowRect.Height / 2,
			//				  y: arrowRect.Y - arrowRect.Height / 2 + defaultIndicatorBounds.Height / 2),
			//	color: Color.White
			//);

			//Game1.spriteBatch.DrawString(
			//	spriteFont: Game1.smallFont,
			//	text: "W",
			//	position: new(x: arrowRect.X - arrowRect.Height / 2 - arrowRect.Width - 5f,
			//				  y: arrowRect.Y - arrowRect.Height / 2 + defaultIndicatorBounds.Height / 2),
			//	color: Color.White
			//);
		}

		private void RotateCompass(NPC npc, Rectangle arrowRect) {
			GameLocation playerLocation = PlayerLocation();
			GameLocation npcLocation = FindLocationOf(npc);

			currentNPCIsNotInLocation = playerLocation == npcLocation;
			if (currentNPCIsNotInLocation)
				_rotation = CompassRotationTo(npc);
			else {
				DrawNPCLocationAboveCompass(npcLocation, arrowRect);
				_rotation = 0;
			}
		}

		private static float CompassRotationTo(NPC npc) {
			// Utility.ForAllLocations
			// Utility.drawLineWithScreenCoordinates
			Vector2 vector = Game1.player.position;
			Vector2 vector2 = npc.position;
			Vector2 vector3 = vector - vector2;
			return (float)Math.Atan2(vector3.Y, vector3.X) - MathF.PI / 2;
		}

		private static GameLocation PlayerLocation() {
			return Game1.player.currentLocation;
		}

		private static GameLocation FindLocationOf(NPC npc) {
			return npc.currentLocation;
		}

		private static void DrawNPCLocationAboveCompass(GameLocation loc, Rectangle arrowRect) {
			Vector2 center = Utility.PointToVector2(arrowRect.Center);
			float xOff = -arrowRect.Width * 0.75f - 100;
			float yOff = -arrowRect.Height * 0.75f - 50;

			string name = loc.NameOrUniqueName;
			const string custom = "Custom_";
			int result = String.Compare(name, 0, custom, 0, 7, true);
			if (result == 0) name = name.Remove(0, 7);

			Game1.spriteBatch.DrawString(
				spriteFont: Game1.smallFont,
				text: name,
				position: new(x: center.X + xOff,
							  y: center.Y - arrowRect.Height + yOff),
				color: Color.White
			);
		}

		private void OnWarped(object? sender, WarpedEventArgs e) {
			if (e.IsLocalPlayer) {
				_pauseTicks.Value = 60;
			}
		}

		private void UpdateTicked(object? sender, UpdateTickedEventArgs e) {
			if (Game1.eventUp || Game1.activeClickableMenu != null)
				return;

			--_pauseTicks.Value;

			// update chat draw. If _pauseTicks is positive then don't render.
			if (e.IsMultipleOf(2) && _pauseTicks.Value <= 0) {
				//_yMovementPerDraw.Value += 0.3f;
				_rotation -= MathF.PI / 30;
				if (_rotation <= -MathF.PI * 2) {
					// reset point
					//_pauseTicks.Value = 60;
					//_yMovementPerDraw.Value = -3f;
					_rotation = 0f;
				}
			}
		}

		public void Dispose() {
			ToggleMod(false);
		}
	}
}
