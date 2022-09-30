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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Xml.Serialization;
using Omegasis.Revitalize.Framework.Hacks;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.StardustCore.UIUtilities;

namespace Omegasis.Revitalize.Framework.World.Objects.Items.Tools
{
    [XmlType("Mods_Revitalize.Framework.World.Objects.Items.Tools.PickaxeExtended")]
    public class PickaxeExtended : Pickaxe, IBasicItemInfoProvider
    {
        public BasicItemInformation info;
        public Texture2DExtended workingTexture;

        public BasicItemInformation basicItemInformation { get => this.info; set => this.info = value; }

        /// <summary>
        /// Used only for accessibility for casting.
        /// </summary>
        [JsonIgnore]
        public BasicItemInformation Info
        {
            get
            {
                return this.info;
            }
            set
            {
                this.info = value;
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


        public PickaxeExtended()
        {

        }

        public PickaxeExtended(BasicItemInformation ItemInfo, int UpgradeLevel, Texture2DExtended WorkingTexture)
        {
            this.info = ItemInfo;
            this.UpgradeLevel = UpgradeLevel;
            this.workingTexture = WorkingTexture;
        }


        public override void draw(SpriteBatch b)
        {
            if (this.lastUser == null || this.lastUser.toolPower <= 0 || !this.lastUser.canReleaseTool)
                return;
            foreach (Vector2 vector2 in this.tilesAffected(this.lastUser.GetToolLocation(false) / 64f, this.lastUser.toolPower, this.lastUser))
                this.info.animationManager.draw(b, Game1.GlobalToLocal(new Vector2((int)vector2.X * 64, (int)vector2.Y * 64)), Color.White, 4f, SpriteEffects.None, 0.01f);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            this.info.animationManager.draw(spriteBatch, location, color * transparency, 4f * scaleSize, SpriteEffects.None, layerDepth);
            //base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
        }


        public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
        {
            ColorChanger.SwapPickaxeTextures(this.workingTexture.texture);
            return base.beginUsing(location, x, y, who);
        }

        public override void actionWhenStopBeingHeld(Farmer who)
        {
            ColorChanger.ResetPickaxeTexture();
            base.actionWhenStopBeingHeld(who);
        }

        public override Color getCategoryColor()
        {
            return this.info.categoryColor;
        }

        public override string getCategoryName()
        {
            return this.info.categoryName;
        }

        public override string getDescription()
        {
            return this.info.description;
        }

        public override Item getOne()
        {
            return new PickaxeExtended(this.info.Copy(), this.UpgradeLevel, this.workingTexture.Copy());
        }
        public override bool canBeTrashed()
        {
            return true;
        }

        public virtual BasicItemInformation getItemInformation()
        {
            return this.info;
        }

        public virtual void setItemInformation(BasicItemInformation Info)
        {
            this.info = Info;
        }
    }
}
