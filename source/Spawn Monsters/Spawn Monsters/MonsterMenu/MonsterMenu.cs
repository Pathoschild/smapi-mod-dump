/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Berisan/SpawnMonsters
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace Spawn_Monsters.MonsterMenu
{
    /// <summary>
    /// Represents a menu for selecting a monster to spawn.
    /// </summary>
    internal class MonsterMenu : IClickableMenu
    {
        private readonly IModHelper modHelper;

        private readonly List<TabComponent> tabComponents;
        private readonly List<IClickableMenu> tabs;
        private int current;

        private const int menuWidth = 800;
        private const int menuHeight = 800;

        public MonsterMenu(IModHelper modHelper)
            : base(
                  Game1.uiViewport.Width / 2 - (menuWidth + IClickableMenu.borderWidth * 2) / 2,
                  Game1.uiViewport.Height / 2 - (menuHeight + IClickableMenu.borderWidth * 2) / 2,
                  menuWidth + IClickableMenu.borderWidth * 2,
                  menuHeight + IClickableMenu.borderWidth * 2,
                  true) {

            this.modHelper = modHelper;

            Game1.playSound("bigSelect");

            tabs = new List<IClickableMenu>();
            tabComponents = new List<TabComponent>();

            tabs.Add(new MonsterSelectionTabNew(modHelper, xPositionOnScreen, yPositionOnScreen, width, height));
            //tabs.Add(new MonsterSettingsTab(xPositionOnScreen, yPositionOnScreen, width, height));

            tabComponents.Add(new TabComponent(new Rectangle(xPositionOnScreen + 64, yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "Monsters"));
            //tabComponents.Add(new TabComponent(new Rectangle(this.xPositionOnScreen + 128, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "Settings"));

            current = 0;
            initializeUpperRightCloseButton();
        }

        public override void draw(SpriteBatch b) {
            base.draw(b);
            if (!Game1.options.showMenuBackground) {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            }

            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, tabs[current].width, tabs[current].height, false, true);
            tabs[current].draw(b);

            for (int i = 0; i < tabComponents.Count; i++) {
                tabComponents[i].Draw(b, i == current);
                if (tabComponents[i].containsPoint(Game1.getMouseX(), Game1.getMouseY())) {
                    drawHoverText(b, tabComponents[i].name, Game1.smallFont);
                }
            }
            drawMouse(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            base.receiveLeftClick(x, y, playSound);
            for (int i = 0; i < tabComponents.Count; i++) {
                if (tabComponents[i].containsPoint(x, y)) {
                    if (i != current) {
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
