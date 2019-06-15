using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Spawn_Monsters
{
	/// <summary>
	/// Represents a single monster in one component.
	///</summary>
	class ClickableMonsterComponent : ClickableComponent
	{
		public AnimatedSprite sprite;
		public int StartFrame;
		public int NumberOfFrames;
		public float Interval;
		public object arg;


		public ClickableMonsterComponent(string textureName, int xPosition, int yPosition, int width, int height, int spriteWidth = 16, int spriteHeight = 24, int startFrame = 0, int numberOfFrames = 4, float interval = 100)
			: base(new Microsoft.Xna.Framework.Rectangle(xPosition, yPosition, width, height), textureName)
		{
			sprite = new AnimatedSprite($"Characters\\Monsters\\{textureName}") {
				SpriteHeight = spriteHeight,
				SpriteWidth = spriteWidth
			};
			sprite.UpdateSourceRect();

			StartFrame = startFrame;
			NumberOfFrames = numberOfFrames;
			Interval = interval;
		}

		public void PerformHoverAction(int x, int y) {
			if (containsPoint(x, y)) {
				if (sprite.CurrentAnimation == null) {
					sprite.Animate(Game1.currentGameTime, StartFrame, NumberOfFrames, Interval);
				}
			} else {
				sprite.StopAnimation();
			}
		}

		public void Draw(SpriteBatch b) {
			Point p = bounds.Center;
			sprite.draw(b, new Vector2(p.X - sprite.SpriteWidth, p.Y - sprite.SpriteHeight), 1);
		}

		public void Draw(SpriteBatch b, Color c) {
			Point p = bounds.Center;
			sprite.draw(b, new Vector2(p.X - sprite.SpriteWidth, p.Y - sprite.SpriteHeight), 1, 0, 0, c, false, 4);
		}
	}
}
