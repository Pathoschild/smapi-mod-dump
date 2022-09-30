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
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.Utilities.Extensions;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.StardustCore.Animations;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects
{
    [XmlType("Mods_Revitalize.Framework.World.Objects.CustomItem")]
    public class CustomItem : StardewValley.Object, ICustomModObject
    {
        public readonly NetRef<BasicItemInformation> netBasicItemInformation = new NetRef<BasicItemInformation>();

        [XmlIgnore]
        public BasicItemInformation basicItemInformation
        {
            get
            {
                return this.netBasicItemInformation.Value;
            }
            set
            {
                this.netBasicItemInformation.Value = value;
            }
        }

        [XmlIgnore]
        public string Id
        {
            get
            {
                return this.basicItemInformation.id.Value;
            }
        }

        [XmlIgnore]
        public AnimationManager AnimationManager
        {
            get
            {
                if (this.basicItemInformation == null) return null;
                if (this.basicItemInformation.animationManager == null) return null;
                return this.basicItemInformation.animationManager;
            }
        }

        [XmlIgnore]
        public Texture2D CurrentTextureToDisplay
        {

            get
            {
                if (this.AnimationManager == null) return null;
                return this.AnimationManager.getTexture();
            }
        }

        public override string Name
        {
            get
            {
                if (this.basicItemInformation == null) return null;
                return this.basicItemInformation.name.Value;
            }
            set
            {
                if (this.basicItemInformation != null)
                    this.basicItemInformation.name.Value = value;
            }


        }
        public override string DisplayName
        {
            get
            {
                if (this.basicItemInformation == null) return null;
                return this.basicItemInformation.name.Value;
            }
            set
            {
                if (this.basicItemInformation != null)
                    this.basicItemInformation.name.Value = value;
            }
        }

        public CustomItem()
        {
            this.basicItemInformation = new BasicItemInformation();
            this.initNetFieldsPostConstructor();

        }

        public CustomItem(BasicItemInformation info) : base(Vector2.Zero, 0)
        {
            this.basicItemInformation = info;
            this.initNetFieldsPostConstructor();

        }

        /// <summary>
        /// When this item is clicked in the world.
        /// </summary>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool clicked(Farmer who)
        {
            //Game1.showRedMessage("Clicked");
            return base.clicked(who);
        }

        /// <summary>
        /// When item is using the right click input.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override bool performUseAction(GameLocation location)
        {
            //  Game1.showRedMessage("Use action");
            return base.performUseAction(location);
        }

        /// <summary>
        /// When placed into the world.
        /// </summary>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool performDropDownAction(Farmer who)
        {
            //   Game1.showRedMessage("DropDown");
            return base.performDropDownAction(who);
        }

        /// <summary>
        /// Used when something is dropped into this object, such as fruit into a keg.
        /// </summary>
        /// <param name="dropInItem"></param>
        /// <param name="probe"></param>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
        {
            // Game1.showRedMessage("Drop-in");
            return base.performObjectDropInAction(dropInItem, probe, who);
        }

        /// <summary>
        /// When a tool is used on this placed object.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public override bool performToolAction(Tool t, GameLocation location)
        {
            // Game1.showRedMessage("Tool action");
            return base.performToolAction(t, location);
        }

        /// <summary>
        /// When removed from world
        /// </summary>
        /// <param name="tileLocation"></param>
        /// <param name="environment"></param>
        public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
        {
            // Game1.showRedMessage("Remove action");
            base.performRemoveAction(tileLocation, environment);
        }

        /// <summary>
        /// Initializes NetFields to send information for multiplayer after all of the constructor initialization for this object has taken place.
        /// </summary>
        protected virtual void initNetFieldsPostConstructor()
        {
            this.NetFields.AddFields(this.netBasicItemInformation);

        }

        public override Color getCategoryColor()
        {
            return this.basicItemInformation.categoryColor.Value;
        }

        public override string getCategoryName()
        {
            return this.basicItemInformation.categoryName.Value;
        }

        public override string getDescription()
        {
            return this.basicItemInformation.description.Value;
        }


        public override Item getOne()
        {
            return new CustomItem(this.basicItemInformation.Copy());
        }

        public override void drawAsProp(SpriteBatch b)
        {
            base.drawAsProp(b);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            this.DrawICustomModObjectInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
        }

        public override void drawAttachments(SpriteBatch b, int x, int y)
        {
            base.drawAttachments(b, x, y);
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            this.DrawICustomModObjectWhenHeld(spriteBatch, objectPosition, f);
        }

        public virtual void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f, float Transparency, float Scale)
        {
            this.DrawICustomModObjectWhenHeld(spriteBatch, objectPosition, f, Transparency, Scale);
        }

        /// <summary>What happens when the object is drawn at a tile location.</summary>
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            this.DrawICustomModObject(spriteBatch, alpha);
        }

        public override void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location)
        {
            //Need to update this????
            base.drawPlacementBounds(spriteBatch, location);
        }

        /// <summary>Draw the game object at a non-tile spot. Aka like debris.</summary>
        public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1f)
        {

            if (this.AnimationManager == null)
                spriteBatch.Draw(this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2(xNonTile, yNonTile)), new Rectangle?(this.AnimationManager.getCurrentAnimationFrameRectangle()), this.basicItemInformation.DrawColor * alpha, 0f, Vector2.Zero, Game1.pixelZoom, this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, layerDepth));

            else
                //Log.AsyncC("Animation Manager is working!");
                this.AnimationManager.draw(spriteBatch, this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2(xNonTile, yNonTile)), new Rectangle?(this.AnimationManager.getCurrentAnimationFrameRectangle()), this.basicItemInformation.DrawColor * alpha, 0f, Vector2.Zero, Game1.pixelZoom, this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, layerDepth));
        }

        public override int maximumStackSize()
        {
            return 999;
        }

        public override bool isPlaceable()
        {
            return false;
        }

        public override void dropItem(GameLocation location, Vector2 origin, Vector2 destination)
        {
            WorldUtilities.WorldUtility.CreateItemDebrisAtTileLocation(location, this, origin / Game1.tileSize, destination / Game1.tileSize);
        }
    }
}
