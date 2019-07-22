using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace Spawn_Monsters
{
	/// <summary>
	/// Represents a button used in MultiClickableMonsterComponents.
	/// </summary>
	class ColoredButtonComponent : ClickableComponent
	{
		public Color color;
		public int index;

		public ColoredButtonComponent(int xPos, int yPos, int width, int height, Color color, int index)
			: base(new Rectangle(xPos, yPos, width, height), "name") {
			this.color = color;
			this.index = index;
		}
		public void Draw(SpriteBatch b) {
			b.Draw(StardewValley.Game1.staminaRect, bounds, color);
		}
	}
}
