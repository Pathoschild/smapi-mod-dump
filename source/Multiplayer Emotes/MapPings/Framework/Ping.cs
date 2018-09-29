using MapPings.Framework.Constants;
using MapPings.Framework.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapPings.Framework {

	// TODO: Extend from ClickableAnimatedComponent
	public class Ping : IClickableMenu {
		private float totalTimer = 0f;

		public Farmer Player { get; set; }
		public Vector2 Position { get; set; }
		public string LocationName { get; set; }
		public Color PingColor { get; set; }

		public bool AnimateWave { get; set; }

		public TemporaryAnimatedSprite AnimatedArrowSprite { get; set; }
		public TemporaryAnimatedSprite AnimatedWaveSprite { get; set; }

		public Ping(Farmer player, Vector2 position, string locationName) : this(player, position, locationName, Color.White) {
		}

		public Ping(Farmer player, Vector2 position, string locationName, Color color) {

			this.Player = player;
			this.Position = position;
			this.LocationName = locationName;
			this.PingColor = color;

			Vector2 mapPos = Utility.getTopLeftPositionForCenteringOnScreen(Sprites.Map.SourceRectangle.Width * Game1.pixelZoom, Sprites.Map.SourceRectangle.Height * Game1.pixelZoom, 0, 0);

			Vector2 pingArrowPos = new Vector2((mapPos.X + Position.X) - (Sprites.VanillaPingArrow.SourceRectangle.Width * Game1.pixelZoom) / 2, (mapPos.Y + Position.Y) - 8 - Sprites.VanillaPingArrow.SourceRectangle.Height * Game1.pixelZoom);
			// // Original texture
			//AnimatedIcon = new TemporaryAnimatedSprite(Sprites.VanillaPingArrow.AssetName, Sprites.VanillaPingArrow.SourceRectangle, 90f, 6, 999999, pingArrowPos, false, false, 0.89f, 0.0f, color, 4f, 0.0f, 0.0f, 0.0f, true) {
			//	yPeriodic = true,
			//	yPeriodicLoopTime = 1500f,
			//	yPeriodicRange = 8f
			//};
			AnimatedArrowSprite = new TemporaryAnimatedSprite(Sprites.VanillaPingArrow.AssetName, Sprites.PingArrow.SourceRectangle, 90f, 6, 999999, pingArrowPos, false, false, 0.89f, 0.0f, color, 4f, 0.0f, 0.0f, 0.0f, true) {
				texture = Sprites.PingArrow.Texture(ModEntry.ModHelper),
				yPeriodic = true,
				yPeriodicLoopTime = 1500f,
				yPeriodicRange = 8f
			};

			Vector2 pingWavePos = new Vector2((mapPos.X + Position.X) - (Sprites.VanillaPingArrow.SourceRectangle.Width * Game1.pixelZoom) / 2, (mapPos.Y + Position.Y) - 8 - Sprites.VanillaPingArrow.SourceRectangle.Height * Game1.pixelZoom);
			AnimatedWaveSprite = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 0, 64, 64), 150f, 8, 0, pingWavePos, false, Game1.random.NextDouble() < 0.5, 0.001f, 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f, true);

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
			totalTimer += time.ElapsedGameTime.Milliseconds;
			float yPos = AnimatedArrowSprite.initialPosition.Y + AnimatedArrowSprite.yPeriodicRange * (float)Math.Sin(2.0 * Math.PI / (double)AnimatedArrowSprite.yPeriodicLoopTime * (double)(totalTimer + AnimatedArrowSprite.yPeriodicLoopTime / 2f));
			ModEntry.ModMonitor.Log($"Y pos: {yPos}");
		}

		public override void draw(SpriteBatch b) {
			AnimatedArrowSprite.draw(b);
			if(AnimateWave) {
				AnimatedWaveSprite.draw(b);
			}
		}

	}

}
