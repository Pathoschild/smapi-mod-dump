using MapPings.Framework.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;

namespace MapPings.Framework {

	// TODO: Extend from ClickableAnimatedComponent
	public class Ping : IClickableMenu {

		IReflectedField<float> totalTimerField;

		public Farmer Player { get; set; }
		public Vector2 Position { get; set; }
		public string LocationName { get; set; }
		public Color PingColor { get; set; }

		public bool AnimateWave { get; set; }

		public TemporaryAnimatedSprite AnimatedArrowSprite { get; set; }
		private readonly float arrowSpriteLoopTime = 1500f;
		private readonly float arrowSpritePeriodicRange = 8f;
		float lastYPos;

		public TemporaryAnimatedSprite AnimatedWaveSprite { get; set; }

		public Ping(Farmer player, Vector2 position, string locationName) : this(player, position, locationName, Color.White) {
		}

		public Ping(Farmer player, Vector2 position, string locationName, Color color) {

			this.Player = player;
			this.Position = position;
			this.LocationName = locationName;
			this.PingColor = color;

			Vector2 mapPos = Utility.getTopLeftPositionForCenteringOnScreen(Sprites.Map.SourceRectangle.Width * Game1.pixelZoom, Sprites.Map.SourceRectangle.Height * Game1.pixelZoom, 0, 0);

			Vector2 pingArrowPos = new Vector2((mapPos.X + Position.X) - (Sprites.VanillaPingArrow.SourceRectangle.Width * Game1.pixelZoom) / 2, (mapPos.Y + Position.Y) - arrowSpritePeriodicRange - Sprites.VanillaPingArrow.SourceRectangle.Height * Game1.pixelZoom);
			// // Original texture
			//AnimatedIcon = new TemporaryAnimatedSprite(Sprites.VanillaPingArrow.AssetName, Sprites.VanillaPingArrow.SourceRectangle, 90f, 6, 999999, pingArrowPos, false, false, 0.89f, 0.0f, color, 4f, 0.0f, 0.0f, 0.0f, true) {
			//	yPeriodic = true,
			//	yPeriodicLoopTime = 1500f,
			//	yPeriodicRange = 8f
			//};
			AnimatedArrowSprite = new TemporaryAnimatedSprite(Sprites.VanillaPingArrow.AssetName, Sprites.PingArrow.SourceRectangle, 90f, 6, 999999, pingArrowPos, false, false, 0.89f, 0.0f, color, 4f, 0.0f, 0.0f, 0.0f, true) {
				texture = Sprites.PingArrow.Texture(ModEntry.ModHelper),
				yPeriodic = true,
				yPeriodicLoopTime = arrowSpriteLoopTime,
				yPeriodicRange = arrowSpritePeriodicRange
			};

			totalTimerField = ModEntry.ModHelper.Reflection.GetField<float>(AnimatedArrowSprite, "totalTimer");

			Vector2 pingWavePos = new Vector2((mapPos.X + Position.X) - (Sprites.VanillaPingArrow.SourceRectangle.Width * Game1.pixelZoom) / 2, (mapPos.Y + Position.Y) - Sprites.VanillaPingArrow.SourceRectangle.Height * Game1.pixelZoom);
			AnimatedWaveSprite = new TemporaryAnimatedSprite(Sprites.PingWave.AssetName, Sprites.PingWave.SourceRectangle, 150f, 8, 0, pingWavePos, false, Game1.random.NextDouble() < 0.5, 0.001f, 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f, true);

		}

		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
			UpdatePingPosition();
		}

		public void UpdatePingPosition() {
			Vector2 mapPos = Utility.getTopLeftPositionForCenteringOnScreen(Sprites.Map.SourceRectangle.Width * Game1.pixelZoom, Sprites.Map.SourceRectangle.Height * Game1.pixelZoom);
			Vector2 pingArrowPos = new Vector2((mapPos.X + Position.X) - (Sprites.VanillaPingArrow.SourceRectangle.Width * Game1.pixelZoom) / 2, (mapPos.Y + Position.Y) - Sprites.VanillaPingArrow.SourceRectangle.Height * Game1.pixelZoom);

			AnimatedArrowSprite.initialPosition = pingArrowPos;
			AnimatedArrowSprite.Position = pingArrowPos;
		}

		public override void update(GameTime time) {
			AnimatedArrowSprite.update(time);


			double sinValue = Math.Sin(2.0 * Math.PI / AnimatedArrowSprite.yPeriodicLoopTime * (totalTimerField.GetValue() + AnimatedArrowSprite.yPeriodicLoopTime / 2f));
			double sVal = Math.Sin(AnimatedArrowSprite.yPeriodicLoopTime / 2f);
			float yPos = AnimatedArrowSprite.initialPosition.Y + AnimatedArrowSprite.yPeriodicRange * (float)Math.Sin(2.0 * Math.PI / AnimatedArrowSprite.yPeriodicLoopTime * (totalTimerField.GetValue() + AnimatedArrowSprite.yPeriodicLoopTime / 2f));

			// TODO: When sin is in quadrant 3 play once wave animation. Note: Check in wich quadrand is.
			if(sinValue > -1 && yPos > lastYPos) {
				//ModEntry.ModMonitor.Log($"Y pos: {yPos}");

				ModEntry.ModMonitor.Log($"Sin: {sinValue}");
			} else if(sinValue < 1) {
				lastYPos = yPos;
			}
		}

		public override void draw(SpriteBatch b) {
			AnimatedArrowSprite.draw(b);
			if(AnimateWave) {
				AnimatedWaveSprite.draw(b);
			}
		}

	}

}
