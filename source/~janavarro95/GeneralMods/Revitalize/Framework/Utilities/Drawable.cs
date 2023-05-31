/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Omegasis.Revitalize.Framework.World.Objects;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.StardustCore.Networking;
using StardewValley;

namespace Omegasis.Revitalize.Framework.Utilities
{
    /// <summary>
    /// Used to draw various items and objects at non tile locations such as in menus or even on other objects.
    /// </summary>
    [XmlType("Mods_Omegasis.Revitalize.Framework.Utilities.Drawable")]
    public class Drawable: NetObject
    {
        public readonly NetRef<StardewValley.Item> item = new NetRef<StardewValley.Item>();

        public Drawable()
        {

        }

        public Drawable(StardewValley.Item item)
        {
            this.item.Value = item;
        }

        protected override void initializeNetFields()
        {
            base.initializeNetFields();
            this.NetFields.AddFields(this.item);
        }

        public virtual void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (this.item.Value != null)
            {
                this.item.Value.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
            }
        }

        public virtual void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            if (this.item.Value != null)
            {
                if(this.item.Value is StardewValley.Object)
                {
                    StardewValley.Object obj =(StardewValley.Object) this.item.Value;
                    obj.drawWhenHeld(spriteBatch, objectPosition, f);
                }
            }
        }

        public virtual void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f, float transparency, float Scale)
        {
            if (this.item.Value != null)
            {
                if (this.item.Value is StardewValley.Object && !(this.item.Value is ICustomModObject))
                {
                    StardewValley.Object obj = (StardewValley.Object)this.item.Value;
                    obj.drawWhenHeld(spriteBatch, objectPosition, f);
                }
                if (this.item.Value is StardewValley.Object && this.item.Value is ICustomModObject)
                {
                    ICustomModObject obj = (ICustomModObject)this.item.Value;
                    obj.drawWhenHeld(spriteBatch, objectPosition, f,transparency,Scale);
                }
            }
        }

        /// <summary>
        /// Draws an object in the game world.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="alpha"></param>
        public virtual void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            if (this.item.Value != null && this.item.Value is StardewValley.Object)
            {
                StardewValley.Object obj = (StardewValley.Object)this.item.Value;
                obj.draw(spriteBatch, x, y, alpha);
            }
        }

        public virtual void draw(SpriteBatch spriteBatch, int x, int y, float depth ,float alpha = 1)
        {
            if (this.item.Value != null && this.item.Value is StardewValley.Object)
            {
                StardewValley.Object obj = (StardewValley.Object)this.item.Value;
                obj.draw(spriteBatch, x, y, depth ,alpha);
            }
        }

        /// <summary>
        /// Draws this give
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="itemToDraw"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="alpha"></param>
        /// <param name="AddedDepth"></param>
        public virtual void drawAsHeldObject( SpriteBatch spriteBatch ,Vector2 TileLocation, float alpha = 1f, float AddedDepth=0f)
        {
            SpriteBatchUtilities.DrawHeldObject(spriteBatch, this.item, TileLocation, alpha, AddedDepth);
        }

        public virtual Drawable Copy()
        {
            if (this.item.Value != null)
            {
                return new Drawable(this.item.Value.getOne());
            }
            return new Drawable();
        }
    }
}
