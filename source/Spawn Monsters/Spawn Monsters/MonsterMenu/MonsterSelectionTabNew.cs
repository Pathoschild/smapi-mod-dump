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
using Spawn_Monsters.Monsters;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace Spawn_Monsters.MonsterMenu
{
    internal class MonsterSelectionTabNew : IClickableMenu
    {
        private readonly List<ClickableComponent> clickableComponents = new List<ClickableComponent>();
        private readonly int maxColums = 20;
        private readonly int maxRows = 20;
        private readonly int defaultTextureWidth = 8 * 4; //Account for texture scaling
        private readonly int defaultTextureHeight = 8 * 4;
        private readonly int clearing = 8;

        public MonsterSelectionTabNew(int x, int y, int width, int height) :
            base(x, y, width, height) {
            LayoutMonsters(MonsterData.ToClickableMonsterComponents());
        }

        private void LayoutMonsters(List<ClickableMonsterComponent> components) {
            int sideBorderClearance = xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth - 16;
            int topBorderClearance = yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - 16;

            ClickableMonsterComponent[,] newPageLayout = new ClickableMonsterComponent[maxColums, maxRows]; //Every field should be 8 * 8
            int colum = 0;
            int row = 0;

            foreach (ClickableMonsterComponent component in components) {

                while (SpaceOccupied(newPageLayout, colum, row, component)) {

                    ++colum;
                    if (colum >= maxColums) {
                        colum = 0;
                        ++row;
                        if (row >= maxRows) {
                            throw (new Exception("We couldnt fit all monsters on the page! This should not happen!"));
                        }
                    }
                }

                //Block space:
                if (component.HeightLevel >= 2) { //16x16
                    newPageLayout[colum, row] = component;
                    newPageLayout[colum + 1, row] = component;
                    newPageLayout[colum, row + 1] = component;
                    newPageLayout[colum + 1, row + 1] = component;

                    if (component.HeightLevel >= 3) { //16x24
                        newPageLayout[colum, row + 2] = component;
                        newPageLayout[colum + 1, row + 2] = component;

                        if (component.HeightLevel >= 4) {//16x32
                            newPageLayout[colum, row + 3] = component;
                            newPageLayout[colum + 1, row + 3] = component;

                            if (component.WidthLevel >= 4) {//32x32
                                newPageLayout[colum + 2, row] = component;
                                newPageLayout[colum + 2, row + 1] = component;
                                newPageLayout[colum + 2, row + 2] = component;
                                newPageLayout[colum + 2, row + 3] = component;

                                newPageLayout[colum + 3, row] = component;
                                newPageLayout[colum + 3, row + 1] = component;
                                newPageLayout[colum + 3, row + 2] = component;
                                newPageLayout[colum + 3, row + 3] = component;
                            }
                        }
                    }
                }
                component.bounds = new Rectangle(sideBorderClearance + colum * (defaultTextureWidth + clearing), topBorderClearance + row * (defaultTextureHeight + clearing), component.bounds.Width, component.bounds.Height);
                clickableComponents.Add(component);
                colum = 0;
                row = 0;
            }
        }

        private bool SpaceOccupied(ClickableMonsterComponent[,] pageLayout, int x, int y, ClickableMonsterComponent component) {
            //Height Level 1: 8x8
            if (pageLayout[x, y] != null) {
                return true;
            }

            //Height Level 2: 16x16
            if (component.HeightLevel >= 2) {
                if (y < maxRows - 1 && x < maxColums - 1) {
                    if (pageLayout[x + 1, y] != null || pageLayout[x, y + 1] != null || pageLayout[x + 1, y + 1] != null) {
                        return true;
                    } else {
                        //Height Level 3: 16x24
                        if (component.HeightLevel >= 3) {
                            if (y < maxRows - 2) {
                                if (pageLayout[x, y + 2] != null || pageLayout[x + 1, y + 2] != null) {
                                    return true;
                                } else {
                                    //Height Level 4: 16x32
                                    if (component.HeightLevel >= 4) {
                                        if (y < maxRows - 3) {
                                            if (pageLayout[x, y + 3] != null || pageLayout[x + 1, y + 3] != null) {
                                                return true;
                                            } else {
                                                //Width Level 4: 32x32
                                                if (component.WidthLevel >= 4) {
                                                    if (x < maxColums - 3) {
                                                        if (pageLayout[x + 2, y] != null || pageLayout[x + 2, y + 1] != null || pageLayout[x + 2, y + 2] != null || pageLayout[x + 2, y + 3] != null
                                                            || pageLayout[x + 3, y] != null || pageLayout[x + 3, y + 1] != null || pageLayout[x + 3, y + 2] != null || pageLayout[x + 3, y + 3] != null) {
                                                            return true;
                                                        } else {
                                                            return false;
                                                        }
                                                    } else {
                                                        return true;
                                                    }
                                                } //Break here, component is 16x32
                                            }
                                        } else {
                                            return true;
                                        }
                                    } // Break here, component is 16x24
                                }
                            } else {
                                return true;
                            }
                        } //Break here, component is 16x16
                    }
                } else {
                    return true;
                }
            } //Break here, component is 8x8 (doesnt exist, but just to be consistent)

            return false;
        }

        public override void draw(SpriteBatch b) {
            base.draw(b);
            foreach (ClickableMonsterComponent component in clickableComponents) {
                component.Draw(b);
            }
        }

        public override void performHoverAction(int x, int y) {
            base.performHoverAction(x, y);
            foreach (ClickableComponent component in clickableComponents) {
                if (component.GetType() == typeof(ClickableMonsterComponent)) {
                    ClickableMonsterComponent m = (ClickableMonsterComponent)component;
                    m.PerformHoverAction(x, y);
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            if (isWithinBounds(x, y)) {

                foreach (ClickableComponent monster in clickableComponents) {
                    if (monster.GetType() == typeof(ClickableMonsterComponent) && monster.containsPoint(x, y)) {
                        ClickableMonsterComponent m = (ClickableMonsterComponent)monster;
                        Game1.activeClickableMenu = new MonsterPlaceMenu(m.Monster, m.Sprite);
                    }
                }
                base.receiveLeftClick(x, y, true);

            } else {
                Game1.exitActiveMenu();
            }
        }
    }
}
