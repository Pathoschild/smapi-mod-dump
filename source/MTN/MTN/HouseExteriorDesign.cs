using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN
{
    /// <summary>
    /// Experimental class for custom house exterior designs.
    /// </summary>
    public class HouseExteriorDesign
    {
        public string name;
        public int woodRequired;
        public int stoneRequired;
        public int copperRequired;
        public int IronRequired;
        public int GoldRequired;
        public int IridiumRequired;
        public int tilesWidth;
        public int tilesHeight;
        public int maxOccupants;
        public int moneyRequired;
        public int daysToConstruct;
        public string description;

        public string textureName;
        public Texture2D texture;

        public HouseExteriorDesign(string name, string description)
        {
            this.name = name;
            texture = Memory.instance.Helper.Content.Load<Texture2D>(Path.Combine("Building", name, "houses.png"), ContentSource.ModFolder);
            moneyRequired = 1000;
        }

        public void consumeResources()
        {
            //Just money for now.
            Game1.player.Money -= moneyRequired;
        }

        public bool doesFarmerHaveEnoughResourcesToBuild()
        {
            //Just money for now
            return Game1.player.Money >= moneyRequired;
        }

        public void drawDescription(SpriteBatch b, int x, int y, int width)
        {
            b.DrawString(Game1.smallFont, this.name, new Vector2((float)x, (float)y), Game1.textColor);
            string text = Game1.parseText(this.description, Game1.smallFont, width);
            b.DrawString(Game1.smallFont, text, new Vector2((float)x, (float)y + Game1.smallFont.MeasureString(this.name).Y), Game1.textColor * 0.75f);
            int num1 = (int)((double)y + (double)Game1.smallFont.MeasureString(this.name).Y + (double)Game1.smallFont.MeasureString(text).Y);

            if (this.moneyRequired <= 0)
                return;
            b.Draw(Game1.debrisSpriteSheet, new Vector2((float)x, (float)num1), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, -1, -1)), Color.White, 0.0f, new Vector2(24f, 11f), 0.5f, SpriteEffects.None, 0.999f);
            Color color1 = Game1.player.money >= this.moneyRequired ? Color.DarkGreen : Color.DarkRed;
            b.DrawString(Game1.smallFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)this.moneyRequired), new Vector2((float)(x + 16 + 8), (float)num1), color1);
            int num2 = num1 + (int)Game1.smallFont.MeasureString(string.Concat((object)this.moneyRequired)).Y;
        }
    }
}
