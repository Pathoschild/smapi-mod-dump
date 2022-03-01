/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Tools;
using SObject = StardewValley.Objects;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Util;


namespace ItemPipes.Framework.Items.Objects
{
    [XmlType("Mods_sergiomadd.ItemPipes_WrenchItem")]

    public class WrenchItem : Tool
    {
        public string IDName { get; set; }
        public string Description { get; set; }

        [XmlIgnore]
        public Texture2D SpriteTexture { get; set; }
        public string SpriteTexturePath { get; set; }
        [XmlIgnore]
        public Texture2D ItemTexture { get; set; }
        public string ItemTexturePath { get; set; }
        public WrenchItem() : base()
        {
            Name = "Wrench";
            IDName = "Wrench";
            Description = "Teh tool for enabling/disabling IOPipes";
            ItemTexturePath = $"assets/{IDName}_Item.png";
            ItemTexture = ModEntry.helper.Content.Load<Texture2D>(ItemTexturePath);
            this.BaseName = Name;
            this.Stackable = false;
            this.numAttachmentSlots.Value = 0;
            this.InstantUse = true;
        }

        public override void leftClick(Farmer who)
        {

        }

        public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
        {
            Game1.toolAnimationDone(who);
            who.canReleaseTool = false;
            who.UsingTool = false;
            who.CanMove = true;
            return true;
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
        {
            int tileX = x / 64;
            int tileY = y / 64;
            StardewValley.Object obj = location.getObjectAtTile(tileX, tileY);
            if(obj != null)
            {
                if(obj is IOPipeItem)
                {
                    IOPipeItem pipe = (IOPipeItem)obj;
                    pipe.ChangeSignal();
                    location.playSound("smallSelect");
                }
            }
        }
        
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(ItemTexture, location + new Vector2(32f, 32f), Game1.getSquareSourceRectForNonStandardTileSheet(Game1.toolSpriteSheet, 16, 16, this.IndexOfMenuItemView), color * transparency, 0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);
        }

        public override void draw(SpriteBatch b)
        {
            if (this.lastUser == null || this.lastUser.toolPower <= 0 || !this.lastUser.canReleaseTool)
            {
                return;
            }
            //Hacer que tenga animacion?
            /*
            foreach (Vector2 v in this.tilesAffected(this.lastUser.GetToolLocation() / 64f, this.lastUser.toolPower, this.lastUser))
            {
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2((int)v.X * 64, (int)v.Y * 64)), new Rectangle(194, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01f);
            }
            */
        }


        public override bool CanAddEnchantment(BaseEnchantment enchantment)
        {
            return false;
        }

        public override Item getOne()
        {
            return new WrenchItem();
        }
        public override string getCategoryName()
        {
            return "Item Pipes";
        }
        public override Color getCategoryColor()
        {
            return Color.Black;
        }

        public override string getDescription()
        {
            return Description;
        }

        protected override string loadDescription()
        {
            return Description;
        }

        protected override string loadDisplayName()
        {
            return Name;
        }
    }
}
