using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    public class MenuTabItem : OptionsElement
    {
        private const int SPRITE_SIZE = 64;
        private int ParentSheetIndex;
        private SObject Item;
        private string Name;
        public string HoverText;

        public Rectangle HoverTextBounds;

        public MenuTabItem(SObject obj) : base("")
        {
            Item = obj;
            ParentSheetIndex = obj.ParentSheetIndex;
            Initialize();
        }

        private void Initialize()
        {
            Name = Item.DisplayName;
            bounds = new Rectangle(8 * Game1.pixelZoom, 4 * Game1.pixelZoom, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom);
            ItemDisplayInfo howToObtain = new ItemDisplayInfo(Item);
            HoverText = howToObtain.GetItemDisplayInfo();
            HoverTextBounds = new Rectangle();
        }

        public void draw(SpriteBatch b, int slotX, int slotY, int xPositionOnScreen)
        {
            int SpriteX = slotX + bounds.X;
            int SpriteY = slotY + bounds.Y;

            int NameX = slotX + bounds.X + (int)(Game1.tileSize * 1.5);
            int TextY = slotY + bounds.Y + Game1.pixelZoom * 3;

            b.Draw(Game1.objectSpriteSheet, new Rectangle(SpriteX, SpriteY, SPRITE_SIZE, SPRITE_SIZE), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ParentSheetIndex, 16, 16)), Color.White);

            if(Item.Stack > 1)
            {
                Utility.drawTinyDigits(Item.Stack, b, new Vector2(SpriteX + Constants.SPRITE_SIZE - 12, SpriteY + Constants.SPRITE_SIZE - 12), 2f, 0.9f, Color.White);
            }

            DrawQualityStar(Item.Quality, b, new Vector2(SpriteX + 2, SpriteY + Constants.SPRITE_SIZE - 6), 2f);

            SpriteText.drawString(b, Name, NameX, TextY, 999, -1, 999, 1f, 0.1f, false, -1, "", -1);

            if(HoverText != "")
            {
                int InfoX = xPositionOnScreen + Constants.MENU_WIDTH - 56 - SpriteText.getWidthOfString(Constants.TEXT_TO_HOVER_OVER_FOR_INFO);

                HoverTextBounds.X = InfoX;
                HoverTextBounds.Y = TextY;
                HoverTextBounds.Width = SpriteText.getWidthOfString(Constants.TEXT_TO_HOVER_OVER_FOR_INFO);
                HoverTextBounds.Height = SpriteText.getHeightOfString(Constants.TEXT_TO_HOVER_OVER_FOR_INFO);

                SpriteText.drawString(b, Constants.TEXT_TO_HOVER_OVER_FOR_INFO, InfoX, TextY);
            }
        }

        private void DrawQualityStar(int quality, SpriteBatch b, Vector2 coords, float scale)
        {
            switch (quality)
            {
                case Constants.SILVER_QUALITY:
                    Utilities.DrawSilverStar(b, coords, scale);
                    break;
                case Constants.GOLD_QUALITY:
                    Utilities.DrawGoldStar(b, coords, scale);
                    break;
                case Constants.IRIDIUM_QUALITY:
                    Utilities.DrawIridiumStar(b, coords, scale);
                    break;
                default:
                    break;
            }
        }
    }
}
