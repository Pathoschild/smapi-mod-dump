/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ExtraGingerIslandMaps.Patches
{
    public class CustomElevatorMenu : IClickableMenu
    {
        private List<ClickableComponent> Elevators = new List<ClickableComponent>();

        public CustomElevatorMenu()
            : base(0, 0, 0, 0, showUpperRightCloseButton: true)
        {
            var numElevators = 2;
            width = Math.Min(220 + borderWidth * 2, numElevators * 44 + borderWidth * 2); //((numElevators > 50) ? (484 + IClickableMenu.borderWidth * 2) : Math.Min(220 + IClickableMenu.borderWidth * 2, numElevators * 44 + IClickableMenu.borderWidth * 2))
            height = Math.Max(64 + borderWidth * 3, numElevators * 44 / (width - borderWidth) * 44 + 64 + borderWidth * 3);
            xPositionOnScreen = Game1.uiViewport.Width / 2 - width / 2;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - height / 2;
            Game1.playSound("crystal");
            var buttonsPerRow = width / 44 - 1;
            var x = xPositionOnScreen + borderWidth + spaceToClearSideBorder * 3 / 4;
            var y = yPositionOnScreen + borderWidth + borderWidth / 3;
            Elevators.Add(new ClickableComponent(new Rectangle(x, y, 44, 44), 0.ToString() ?? "")
            {
                myID = 0,
                rightNeighborID = 1,
                downNeighborID = buttonsPerRow
            });
            x = x + 64 - 20;
            if (x > xPositionOnScreen + width - borderWidth)
            {
                x = xPositionOnScreen + borderWidth + spaceToClearSideBorder * 3 / 4;
                y += 44;
            }
            /*for (int i = 1; i <= numElevators; i++)
            {	
                this.elevators.Add(new ClickableComponent(new Rectangle(x, y, 44, 44), (i * 5).ToString() ?? "")
                {
                    myID = i,
                    rightNeighborID = ((i % buttonsPerRow == buttonsPerRow - 1) ? (-1) : (i + 1)),
                    leftNeighborID = ((i % buttonsPerRow == 0) ? (-1) : (i - 1)),
                    downNeighborID = i + buttonsPerRow,
                    upNeighborID = i - buttonsPerRow
                });
                x = x + 64 - 20;
                if (x > base.xPositionOnScreen + base.width - IClickableMenu.borderWidth)
                {
                    x = base.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder * 3 / 4;
                    y += 44;
                }
            }
            *
            */

            //add elevator to "mystery zone"
            Elevators.Add(new ClickableComponent(new Rectangle(x, y, 44, 44), "999") //using "???" crashes the game so let's get creative
            {
                myID = 1,
                rightNeighborID = 2,
                leftNeighborID = 0,
                downNeighborID = 1 + buttonsPerRow,
                upNeighborID = 1 - buttonsPerRow
            });

            x = x + 64 - 20;
            if (x > xPositionOnScreen + width - borderWidth)
            {
                x = xPositionOnScreen + borderWidth + spaceToClearSideBorder * 3 / 4;
                y += 44;
            }

            initializeUpperRightCloseButton();
            if (!Game1.options.snappyMenus || !Game1.options.gamepadControls) return;
            base.populateClickableComponentList();
            snapToDefaultClickableComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (isWithinBounds(x, y))
            {
                foreach (var c in Elevators)
                {
                    if (!c.containsPoint(x, y))
                    {
                        continue;
                    }
                    Game1.playSound("smallSelect");
                    if (Convert.ToInt32(c.name) == 0)
                    {
                        if (Game1.currentLocation.Name != "Custom_ExtinctLair")
                        {
                            return;
                        }
                        Game1.warpFarmer("Custom_GiCave", 20, 8, true);
                        Game1.exitActiveMenu();
                        //Game1.changeMusicTrack("none");
                    }
                    else
                    {

                        if ((c.name == "999" && Game1.currentLocation.Name == "Custom_ExtinctLair") || (c.name == "0" && Game1.currentLocation.Name == "Custom_GiCave"))
                        {
                            return;
                        }

                        Game1.player.ridingMineElevator = true;
                        //Game1.enterMine(Convert.ToInt32(c.name));
                        Game1.warpFarmer("Custom_ExtinctLair", 61,91,false);
                        Game1.exitActiveMenu();
                    }
                }
                base.receiveLeftClick(x, y);
            }
            else
            {
                Game1.exitActiveMenu();
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            foreach (var c in Elevators)
            {
                c.scale = c.containsPoint(x, y) ? 2f : 1f;
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen - 64 + 8, width + 21, height + 64, speaker: false, drawOnlyBox: true);
            foreach (var c in Elevators)
            {
                var isCurrentFloor = (c.name == "0" && Game1.currentLocation.Name == "Custom_GiCave") || (c.name == "999" && Game1.currentLocation.Name == "Custom_ExtinctLair");

                b.Draw(Game1.mouseCursors, new Vector2(c.bounds.X - 4, c.bounds.Y + 4), new Rectangle((c.scale > 1f) ? 267 : 256, 256, 10, 10), Color.Black * 0.5f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.865f);

                b.Draw(Game1.mouseCursors, new Vector2(c.bounds.X, c.bounds.Y), new Rectangle((c.scale > 1f) ? 267 : 256, 256, 10, 10), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.868f);

                NumberSprite.draw(position: new Vector2(c.bounds.X + 16 + NumberSprite.numberOfDigits(Convert.ToInt32(c.name)) * 6, c.bounds.Y + 24 - NumberSprite.getHeight() / 4), number: Convert.ToInt32(c.name), b: b, c: isCurrentFloor ? (Color.Gray * 0.75f) : Color.Gold, scale: 0.5f, layerDepth: 0.86f, alpha: 1f, secondDigitOffset: 0);
            }
            base.draw(b);
            drawMouse(b);
        }
    }

}