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
using PyTK.CustomElementHandler;
using Revitalize.Framework.Objects.Interfaces;
using Revitalize.Framework.Utilities;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using StardustCore.UIUtilities;

namespace Revitalize.Framework.Objects.Items.Tools
{
    public class WateringCanExtended:StardewValley.Tools.WateringCan, ISaveElement, IItemInfo, ICustomObject
    {
        public BasicItemInformation info;
        public Texture2DExtended workingTexture;

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

        public Guid guid;

        public override string Name
        {

            get
            {
                if (this.info != null)
                {
                    return this.netName.Value.Split('>')[0];
                    //return this.info.name;
                }
                if (this.netName == null)
                {
                    return this.Name;
                }
                else
                {
                    return this.netName.Value.Split('>')[0]; //Return the value before the name because that is the true value.
                }
            }

            set
            {
                if (this.netName == null)
                {
                    return;
                }
                if (this.netName.Value == null)
                {
                    return;
                }
                if (this.netName.Value.Split('>') is string[] split && split.Length > 1)
                {
                    this.netName.Value = value + ">" + split[1]; //When setting the name if appended data is added on set the new value and add that appended data back.
                }
                else
                {
                    this.netName.Value = value; //Otherwise just set the net name.
                }
            }
        }

        public override string DisplayName
        {
            get
            {
                if (this.info != null)
                {
                    return this.info.name;
                }
                return this.netName.Value.Split('>')[0];
            }

            set
            {
                if (this.netName == null) return;
                if (this.netName.Value == null) return;

                if (this.netName.Value.Split('>') is string[] split && split.Length > 1)
                    this.netName.Value = value + ">" + split[1];
                else
                    this.netName.Value = value;
            }
        }

        public virtual string text
        {
            get
            {
                if (this.netName.Value.Split('>') is string[] split && split.Length > 1)
                    return split[1]; //This is custom data. If the net name has a much larger name split the value and return the result.
                else
                    return ""; //Otherwise return nothing.
            }
            set
            {
                if (this.netName == null) return;
                if (this.netName.Value == null) return;
                {
                    this.netName.Value = this.netName.Value.Split('>')[0] + ">" + value; //When setting the custom dataappend it to the end of the name.
                }
            }
        }

        [JsonIgnore]
        public virtual string ItemInfo
        {
            get
            {
                return Revitalize.ModCore.Serializer.ToJSONString(this.info) + "<" + this.guid + "<" + Revitalize.ModCore.Serializer.ToJSONString(this.workingTexture);
            }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                string[] data = value.Split('<');
                string infoString = data[0];
                string guidString = data[1];
                string WorkingTexture = data[2];

                this.info = (BasicItemInformation)Revitalize.ModCore.Serializer.DeserializeFromJSONString(infoString, typeof(BasicItemInformation));
                Guid oldGuid = this.guid;
                this.guid = Guid.Parse(guidString);
                this.workingTexture = Revitalize.ModCore.Serializer.DeserializeFromJSONString<Texture2DExtended>(WorkingTexture);
                if (ModCore.CustomObjects.ContainsKey(this.guid))
                {
                    //ModCore.log("Update item with guid: " + this.guid);
                    ModCore.CustomItems[this.guid] = this;
                }
                else
                {
                    //ModCore.log("Add in new guid: " + this.guid);
                    ModCore.CustomItems.Add(this.guid, this);
                }

            }
        }

        public WateringCanExtended()
        {

        }

        public WateringCanExtended(BasicItemInformation ItemInfo, int UpgradeLevel, Texture2DExtended WorkingTexture,int WaterCapacity)
        {
            this.info = ItemInfo;
            this.upgradeLevel.Value = UpgradeLevel;
            this.guid = Guid.NewGuid();
            this.workingTexture = WorkingTexture;
            this.waterCanMax = WaterCapacity;
            this.updateInfo();
        }


        public override void draw(SpriteBatch b)
        {
            if (this.lastUser == null || this.lastUser.toolPower <= 0 || !this.lastUser.canReleaseTool)
                return;
            this.updateInfo();
            foreach (Vector2 vector2 in this.tilesAffected(this.lastUser.GetToolLocation(false) / 64f, this.lastUser.toolPower, this.lastUser))
                this.info.animationManager.draw(b, Game1.GlobalToLocal(new Vector2((float)((int)vector2.X * 64), (float)((int)vector2.Y * 64))), Color.White, 4f, SpriteEffects.None, 0.01f);
        }

        public override void drawAttachments(SpriteBatch b, int x, int y)
        {
            this.updateInfo();
            //base.drawAttachments(b, x, y);
            //this.info.animationManager.draw(b,)


        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            this.updateInfo();
            this.info.animationManager.draw(spriteBatch, location, color * transparency, 4f * scaleSize, SpriteEffects.None, layerDepth);
            //base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
        }

        public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
        {
            this.updateInfo();
            Revitalize.Framework.Hacks.ColorChanger.SwapWateringCanTextures(this.workingTexture.texture);
            return base.beginUsing(location, x, y, who);
        }
        public override void endUsing(GameLocation location, Farmer who)
        {
            //Revitalize.Framework.Hacks.ColorChanger.ResetPickaxeTexture();
            base.endUsing(location, who);
        }

        public override bool onRelease(GameLocation location, int x, int y, Farmer who)
        {
            //Revitalize.Framework.Hacks.ColorChanger.ResetPickaxeTexture();
            return base.onRelease(location, x, y, who);
        }

        public override void actionWhenStopBeingHeld(Farmer who)
        {
            Revitalize.Framework.Hacks.ColorChanger.ResetWateringCanTexture();
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
            return new WateringCanExtended(this.info.Copy(), this.UpgradeLevel, this.workingTexture.Copy(),this.waterCanMax);
        }

        /// <summary>
        /// Updates the info on the item.
        /// </summary>
        public virtual void updateInfo()
        {
            if (this.info == null || this.workingTexture == null)
            {
                this.ItemInfo = this.text;
                return;
            }

            if (this.requiresUpdate())
            {
                this.text = this.ItemInfo;
                this.info.cleanAfterUpdate();
                MultiplayerUtilities.RequestUpdateSync(this.guid);
            }
        }

        /// <summary>
        /// Gets an update for this item.
        /// </summary>
        public virtual void getUpdate()
        {
            this.ItemInfo = this.text;
        }

        /// <summary>
        /// Checks to see if this item requires a sync update.
        /// </summary>
        /// <returns></returns>
        public virtual bool requiresUpdate()
        {
            if (this.info.requiresSyncUpdate())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            WateringCanExtended p = Revitalize.ModCore.Serializer.DeserializeGUID<WateringCanExtended>(additionalSaveData["GUID"]);
            return p;
        }
        public Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> serializedInfo = new Dictionary<string, string>();
            serializedInfo.Add("id", this.ItemInfo);
            serializedInfo.Add("ItemInfo", Revitalize.ModCore.Serializer.ToJSONString(this.info));
            serializedInfo.Add("GUID", this.guid.ToString());
            serializedInfo.Add("Level", this.UpgradeLevel.ToString());
            Revitalize.ModCore.Serializer.SerializeGUID(this.guid.ToString(), this);
            return serializedInfo;
        }

        public virtual object getReplacement()
        {
            Chest c = new Chest(true);
            c.playerChoiceColor.Value = Color.Magenta;
            c.TileLocation = new Vector2(0, 0);
            return c;
        }

        public virtual void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            //ModCore.log("REBULD THE PICKAXE!!!!!!!");
            this.info = ModCore.Serializer.DeserializeFromJSONString<BasicItemInformation>(additionalSaveData["ItemInfo"]);
            this.UpgradeLevel = Convert.ToInt32(additionalSaveData["Level"]);
            //this.upgradeLevel.Value = (replacement as Pickaxe).UpgradeLevel;

        }
        public override bool canBeTrashed()
        {
            return true;
        }
    }
}
