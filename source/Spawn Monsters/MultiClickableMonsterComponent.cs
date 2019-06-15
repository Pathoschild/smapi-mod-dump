using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace Spawn_Monsters
{
	/// <summary>
	/// Represents multiple monsters in one component.
	/// </summary>
	class MultiClickableMonsterComponent : ClickableComponent
	{
		private List<ClickableMonsterComponent> monsters;
		private List<ColoredButtonComponent> buttons;
		private int current;

		public MultiClickableMonsterComponent(string[] names, Color[] colors, int xPos, int yPos, int width, int height, object[] arguments, int spriteWidth = 16, int spriteHeight = 24) 
			: base(new Microsoft.Xna.Framework.Rectangle(xPos, yPos, width, height), names[0]) {	
			monsters = new List<ClickableMonsterComponent>();
			buttons = new List<ColoredButtonComponent>();
			current = 0;

			//Create monster components and corresponding buttons
			for (int i = 0; i < names.Length; i++) {
				monsters.Add(new ClickableMonsterComponent(names[i], xPos, yPos, width, height, spriteWidth, spriteHeight) {
					arg = (object)arguments[i] //can be null
				});
				int offset = (width - names.Length * 40) / 2 + 20;
				buttons.Add(new ColoredButtonComponent(xPos + i * 40 + offset, yPos + height - 70, 30, 30, colors[i], i));
			}
		}

		public void PerformHoverAction(int x, int y) {
			monsters[current].PerformHoverAction(x, y);
		}

		public void ReceiveLeftClick(int x, int y) {
			//Check if the click landed on this component at all
			if (containsPoint(x, y)) {
				//Check if the click landed on a colored button
				foreach (ColoredButtonComponent c in buttons) {
					if (c.containsPoint(x, y)) {
						current = c.index;
						Game1.playSound("smallSelect");
						return;
					}
				}

				//Otherwise open the menu
				Game1.activeClickableMenu = new MonsterPlaceMenu(monsters[current].name, monsters[current].arg, monsters[current].sprite, monsters[current].name.Equals("Green Slime") || monsters[current].name.Equals("Fly") || monsters[current].name.Equals("Grub") ? buttons[current].color : Color.White);
			}
		}


		public void Draw(SpriteBatch b) {
			//Draw currently selected monster, with color depending on the monster
			if (monsters[current].name.Equals("Green Slime") || monsters[current].name.Equals("Fly") || monsters[current].name.Equals("Grub")) monsters[current].Draw(b, buttons[current].color);
			else monsters[current].Draw(b);

			//Draw the rectangle around the currently selected colored button
			Rectangle r = buttons[current].bounds;
			r.Width += 10;
			r.Height += 10;
			r.X -= 5;
			r.Y -= 5;
			b.Draw(Game1.staminaRect, r, Color.IndianRed);

			//Draw all colored buttons
			foreach(ColoredButtonComponent button in buttons) {
				button.Draw(b);
			}
		}
	}
}
