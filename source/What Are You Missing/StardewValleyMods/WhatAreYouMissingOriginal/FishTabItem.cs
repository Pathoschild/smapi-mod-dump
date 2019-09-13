using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace WhatAreYouMissingOriginal
{
    class FishTabItem : OptionsElement
    {
        private int parentSheetIndex;
        private string name;
        private const int SPRITE_SIZE = 64;
        protected new Rectangle bounds;

        public FishTabItem(int index) 
            : base("What's this do")
        {
            parentSheetIndex = index;
            name = Game1.objectInformation[index].Split('/')[0];
            this.bounds = new Rectangle(8 * Game1.pixelZoom, 4 * Game1.pixelZoom, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom);
        }

        public override void draw(SpriteBatch b, int X, int Y)
        {
            //b.Draw(Game1.objectSpriteSheet, new Rectangle((int)(X + this.bounds.X + Game1.tileSize),Y + this.bounds.Y, SPRITE_SIZE, SPRITE_SIZE), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.parentSheetIndex, 16, 16)), Color.White);
            b.Draw(Game1.objectSpriteSheet, new Rectangle((int)(X + this.bounds.X + Game1.tileSize), Y + this.bounds.Y, SPRITE_SIZE, SPRITE_SIZE), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.parentSheetIndex, 16, 16)), Color.White);
            SpriteText.drawString(b, this.name, X, Y, 999, -1, 999, 1f, 0.1f, false, -1, "", -1);
        }
    }
}