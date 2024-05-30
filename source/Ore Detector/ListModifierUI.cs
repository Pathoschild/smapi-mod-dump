/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/1Avalon/Ore-Detector
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.ItemTypeDefinitions;
using System.Diagnostics.Metrics;

namespace OreDetector
{
    public class ListModifierUI : IClickableMenu
    {
        private List<ClickableTextureComponent> components = new List<ClickableTextureComponent>();

        private List<ClickableTextureComponent> blacklistedCompontents = new List<ClickableTextureComponent>();

        private SaveModel model;

        private List<Point> bigCraftablePositions = new List<Point>();

        private string hoverText;
        public ListModifierUI(ref SaveModel model) 
        {
            int width = Game1.viewport.Width / 2;
            int height = Game1.viewport.Height;
            base.initialize(Game1.viewport.Width / 2 - width / 2, Game1.viewport.Height / 2 - height / 2, width, height);
            this.model = model;
            getTiles();
        }
        private void getTiles()
        {
            List<string> qualifiedObjectIds = model.discoveredmaterialsQualifiedIds;

            int counterX = 0;
            int counterY = 0;
            int offsetX = xPositionOnScreen + 50;
            int offsetY = yPositionOnScreen + 150;
            foreach (string qualifiedObjectId in qualifiedObjectIds)
            {
                int tileWidth = Game1.tileSize;
                int tileHeight = Game1.tileSize;
                ParsedItemData data = ItemRegistry.GetDataOrErrorItem(qualifiedObjectId);
                ClickableTextureComponent component;
                if (counterX % 13 == 0)
                {
                    counterY++;
                    counterX = 0;
                }
                if (qualifiedObjectId.StartsWith("(BC)"))
                {
                    bigCraftablePositions.Add(new Point(counterX, counterY));
                    tileHeight *= 2;
                }
                else if (bigCraftablePositions.Contains(new Point(counterX, counterY - 1)))
                    counterX++;

                component = new ClickableTextureComponent(new Rectangle(offsetX + counterX * 64, offsetY + counterY * 64, tileWidth, tileHeight), data.GetTexture(), data.GetSourceRect(), 4f)
                {
                    name = data.DisplayName
                };
                if (model.blacklistedNames.Contains(data.DisplayName))
                {
                    blacklistedCompontents.Add(component);
                }
                components.Add(component);
                counterX++;
            }
            
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach(ClickableTextureComponent component in components)
            {
                if (component.containsPoint(x, y) && !blacklistedCompontents.Contains(component))
                {
                    blacklistedCompontents.Add(component);
                    model.blacklistedNames.Add(component.name);
                    Game1.playSound("dialogueCharacter");
                    return;
                }
            }
            foreach(ClickableTextureComponent component in blacklistedCompontents)
            {
                if (component.containsPoint(x, y) && blacklistedCompontents.Contains(component))
                {
                    blacklistedCompontents.Remove(component);
                    model.blacklistedNames.Remove(component.name);
                    Game1.playSound("dialogueCharacterClose");
                    return;
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            hoverText = "";
            foreach (ClickableComponent component in components)
            {
                if (component.containsPoint(x, y))
                {
                    hoverText = component.name;
                }
            }
        }
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

            foreach (ClickableTextureComponent component in components)
            {

                if (blacklistedCompontents.Contains(component))
                    component.draw(b, Color.White * 0.3f, 2f);
                else
                    component.draw(b);
            }

            b.DrawString(Game1.dialogueFont, I18n.OreDetector_Blacklist(), new Vector2(xPositionOnScreen + width / 2.5f, yPositionOnScreen + 125), Color.Black);

            drawHoverText(b, hoverText, Game1.smallFont);

            drawMouse(b);
        }
    }
}
