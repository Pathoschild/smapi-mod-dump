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
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using StardewValley;

namespace ItemPipes.Framework.Items.Objects
{
    public class IridiumPipeItem : ConnectorPipeItem
    {
        //Stage 1
        [XmlIgnore]
        public Texture2D ItemTexture1 { get; set; }
        [XmlIgnore]
        public Texture2D SpriteTexture1 { get; set; }
        [XmlIgnore]
        public Texture2D ItemMovingSprite1 { get; set; }
        [XmlIgnore]
        public Texture2D DefaultSprite1 { get; set; }
        [XmlIgnore]
        public Texture2D ConnectingSprite1 { get; set; }

        //Stage 2
        [XmlIgnore]
        public Texture2D ItemTexture2 { get; set; }
        [XmlIgnore]
        public Texture2D SpriteTexture2 { get; set; }
        [XmlIgnore]
        public Texture2D ItemMovingSprite2 { get; set; }
        [XmlIgnore]
        public Texture2D DefaultSprite2 { get; set; }
        [XmlIgnore]
        public Texture2D ConnectingSprite2 { get; set; }

        //Stage 3
        [XmlIgnore]
        public Texture2D ItemTexture3 { get; set; }
        [XmlIgnore]
        public Texture2D SpriteTexture3 { get; set; }
        [XmlIgnore]
        public Texture2D ItemMovingSprite3 { get; set; }
        [XmlIgnore]
        public Texture2D DefaultSprite3 { get; set; }
        [XmlIgnore]
        public Texture2D ConnectingSprite3 { get; set; }

        public IridiumPipeItem() : base()
        {
            LoadStages();
            Init();
        }

        public IridiumPipeItem(Vector2 position) : base(position)
        {
            LoadStages();
            Init();
        }

        public void LoadStages()
        {
            DataAccess DataAccess = DataAccess.GetDataAccess();
            //Stage 1
            ItemTexture1 = DataAccess.Sprites[IDName + "_Item1"];
            DefaultSprite1 = DataAccess.Sprites[IDName + "_default_Sprite1"];
            ConnectingSprite1 = DataAccess.Sprites[IDName + "_connecting_Sprite1"];
            ItemMovingSprite1 = DataAccess.Sprites[IDName + "_item_Sprite1"];
            SpriteTexture1 = DefaultSprite1;

            //Stage 2
            ItemTexture2 = DataAccess.Sprites[IDName + "_Item2"];
            DefaultSprite2 = DataAccess.Sprites[IDName + "_default_Sprite2"];
            ConnectingSprite2 = DataAccess.Sprites[IDName + "_connecting_Sprite2"];
            ItemMovingSprite2 = DataAccess.Sprites[IDName + "_item_Sprite2"];
            SpriteTexture2 = DefaultSprite2;

            //Stage 3
            ItemTexture3 = DataAccess.Sprites[IDName + "_Item3"];
            DefaultSprite3 = DataAccess.Sprites[IDName + "_default_Sprite3"];
            ConnectingSprite3 = DataAccess.Sprites[IDName + "_connecting_Sprite3"];
            ItemMovingSprite3 = DataAccess.Sprites[IDName + "_item_Sprite3"];
            SpriteTexture3 = DefaultSprite3;
        }

        public override void LoadTextures()
        {
            ItemTexture = ItemTexture1;
            SpriteTexture = SpriteTexture1;
            ItemMovingSprite = ItemMovingSprite1;
            DefaultSprite = DefaultSprite1;
            ConnectingSprite = ConnectingSprite1;
        }

        public void StageChange()
        {
            if (((int)Game1.currentGameTime.TotalGameTime.TotalSeconds) % 2 == 0 && ((int)Game1.currentGameTime.TotalGameTime.TotalSeconds) % 3 == 0)
            {
                ItemTexture = ItemTexture3;
                SpriteTexture = SpriteTexture3;
                ItemMovingSprite = ItemMovingSprite3;
                DefaultSprite = DefaultSprite3;
                ConnectingSprite = ConnectingSprite3;
            }
            else if (((int)Game1.currentGameTime.TotalGameTime.TotalSeconds) % 2 == 0)
            {
                ItemTexture = ItemTexture2;
                SpriteTexture = SpriteTexture2;
                ItemMovingSprite = ItemMovingSprite2;
                DefaultSprite = DefaultSprite2;
                ConnectingSprite = ConnectingSprite2;
            }
            else
            {
                ItemTexture = ItemTexture1;
                SpriteTexture = SpriteTexture1;
                ItemMovingSprite = ItemMovingSprite1;
                DefaultSprite = DefaultSprite1;
                ConnectingSprite = ConnectingSprite1;
            }    
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            StageChange();
            base.drawWhenHeld(spriteBatch, objectPosition, f);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            StageChange();
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
        }

        public override void drawAsProp(SpriteBatch b)
        {
            StageChange();
            base.drawAsProp(b);
        }

        public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1)
        {
            StageChange();
            base.draw(spriteBatch, xNonTile, yNonTile, layerDepth, alpha);
        }
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            StageChange();
            base.draw(spriteBatch, x, y, 1);
        }
    }
}
