/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/joisse1101/InstantAnimals
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;

namespace InstantAnimals
{
	public class NewFarmAnimal : FarmAnimal
    {
        [XmlElement("age")]
        public new NetInt age = new NetInt();
        [XmlElement("ageWhenMature")]
        public new NetByte ageWhenMature = new NetByte();
        [XmlElement("type")]
        public new NetString type = new NetString();

        public NewFarmAnimal() : base() { }
        public NewFarmAnimal(string xtype, long id, long ownerID, IDictionary<string, string> data) : base(xtype, id, ownerID) {
			this.ownerID.Value = ownerID;
			this.health.Value = 3;
            this.type.Value = xtype;
            Game1.addHUDMessage(new HUDMessage(String.Format("NewFarmAnimal: {0}", this.type.Value), Color.LimeGreen, 3500f));
			this.myID.Value = id;
            base.Name = Dialogue.randomName();
			base.displayName = base.name;
			this.happiness.Value = byte.MaxValue;
			this.fullness.Value = byte.MaxValue;
			this._nextFollowTargetScan = Utility.RandomFloat(1f, 3f);
			this.reloadData(data);
            Game1.addHUDMessage(new HUDMessage(String.Format("NewFarmAnimalActual: {0}, Mature Age: {1}", this.type.Value, this.ageWhenMature), Color.LimeGreen, 3500f));
        }
		public virtual void reloadData(IDictionary<string, string> data)
		{
			Game1.addHUDMessage(new HUDMessage(data.Keys.ToString(), Color.LimeGreen, 3500f));
			data.TryGetValue(this.type.Value, out var rawData);
			if (rawData != null)
			{
				string[] split = rawData.Split('/');
				this.daysToLay.Value = Convert.ToByte(split[0]);
				this.ageWhenMature.Value = Convert.ToByte(split[1]);
				this.defaultProduceIndex.Value = Convert.ToInt32(split[2]);
				this.deluxeProduceIndex.Value = Convert.ToInt32(split[3]);
				this.sound.Value = (split[4].Equals("none") ? null : split[4]);
				this.frontBackBoundingBox.Value = new Microsoft.Xna.Framework.Rectangle(Convert.ToInt32(split[5]), Convert.ToInt32(split[6]), Convert.ToInt32(split[7]), Convert.ToInt32(split[8]));
				this.sidewaysBoundingBox.Value = new Microsoft.Xna.Framework.Rectangle(Convert.ToInt32(split[9]), Convert.ToInt32(split[10]), Convert.ToInt32(split[11]), Convert.ToInt32(split[12]));
				this.harvestType.Value = Convert.ToByte(split[13]);
				this.showDifferentTextureWhenReadyForHarvest.Value = Convert.ToBoolean(split[14]);
				this.buildingTypeILiveIn.Value = split[15];
				int sourceWidth = Convert.ToInt32(split[16]);
				string textureName = this.type;
				if ((int)this.age < (byte)this.ageWhenMature)
				{
					textureName = "Baby" + (this.type.Value.Substring(4).Equals("Duck") ? "White Chicken" : this.type.Value.Substring(4));
				}
				else if ((bool)this.showDifferentTextureWhenReadyForHarvest && (int)this.currentProduce <= 0)
				{
					textureName = "Sheared" + this.type.Value;
				}
				this.Sprite = new AnimatedSprite("Animals\\" + textureName, 0, sourceWidth, Convert.ToInt32(split[17]));
				this.frontBackSourceRect.Value = new Microsoft.Xna.Framework.Rectangle(0, 0, Convert.ToInt32(split[16]), Convert.ToInt32(split[17]));
				this.sidewaysSourceRect.Value = new Microsoft.Xna.Framework.Rectangle(0, 0, Convert.ToInt32(split[18]), Convert.ToInt32(split[19]));
				this.fullnessDrain.Value = Convert.ToByte(split[20]);
				this.happinessDrain.Value = Convert.ToByte(split[21]);
				this.toolUsedForHarvest.Value = ((split[22].Length > 0) ? split[22] : "");
				this.meatIndex.Value = Convert.ToInt32(split[23]);
				this.price.Value = Convert.ToInt32(split[24]);
				if (!this.isCoopDweller())
				{
					this.Sprite.textureUsesFlippedRightForLeft = true;
				}
			}
			else { throw new InvalidOperationException($"data has no type {this.type.Value}"); }
		}


	}
}
