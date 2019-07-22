using System.Collections.Generic;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spawn_Monsters
{
	/// <summary>
	/// Represents a menu for selecting a monster to spawn.
	/// </summary>
	class MonsterMenu : IClickableMenu
	{

		private List<IClickableMenu> tabs;
		private int current;
		private List<TabComponent> tabComponents;

		public MonsterMenu()
			: base(0, 0, 0, 0, true) {
			this.width = 600;
			this.height = 600;
			this.xPositionOnScreen = Game1.viewport.Width / 2 - this.width / 2;
			this.yPositionOnScreen = Game1.viewport.Height / 2 - this.height / 2;
			Game1.playSound("bigSelect");

			tabs = new List<IClickableMenu>();
			tabComponents = new List<TabComponent>();

			tabs.Add(new MonsterMenuTab());
			tabs.Add(new MonsterSettingsTab());

			tabComponents.Add(new TabComponent(new Rectangle(xPositionOnScreen+30, yPositionOnScreen+16, 64, 64), "Spawn Menu"));
			tabComponents.Add(new TabComponent(new Rectangle(xPositionOnScreen + 94, yPositionOnScreen+16, 64, 64), "Settings"));

			current = 0;
		}

		public override void draw(SpriteBatch b) {
			base.draw(b);

			tabs[current].draw(b);

			for (int i = 0; i < tabComponents.Count; i++) {
				tabComponents[i].Draw(b, i==current);
				if(tabComponents[i].containsPoint(Game1.getMouseX(), Game1.getMouseY())){
					drawHoverText(b, tabComponents[i].name, Game1.smallFont);
				}
			}
			drawMouse(b);
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true) {
			base.receiveLeftClick(x, y, playSound);
			for (int i = 0; i<tabComponents.Count; i++) {
				if (tabComponents[i].containsPoint(x, y)) {
					if(i != current) {
						current = i;
						Game1.playSound("smallSelect");
					}
					return;
				}
			}
			tabs[current].receiveLeftClick(x, y, playSound);
		}

		public override void performHoverAction(int x, int y) {
			base.performHoverAction(x, y);
			tabs[current].performHoverAction(x, y);
		}
	}
}
