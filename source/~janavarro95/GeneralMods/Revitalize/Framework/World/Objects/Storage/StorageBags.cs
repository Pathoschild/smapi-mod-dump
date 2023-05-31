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
using Netcode;
using Omegasis.Revitalize.Framework.Menus.Items;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using StardewValley;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omegasis.Revitalize.Framework.World.Objects.Storage
{
    /// <summary>
    /// A class to handle additional, condensed storage containers.
    /// </summary>
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Storage.StorageBag")]
    public class StorageBag : CustomItem
    {

        public readonly NetInt capacity = new NetInt();

        [XmlIgnore]
        public int Capacity { get => this.capacity.Value; set => this.capacity.Value = value; }

        public readonly NetObjectList<Item> inventory = new NetObjectList<Item>();

        public StorageBag() { }

        public StorageBag(BasicItemInformation basicItemInformation, int Capacity) : base(basicItemInformation)
        {

            this.Capacity = Capacity;
        }


        protected override void initializeNetFieldsPostConstructor()
        {
            base.initializeNetFieldsPostConstructor();
            this.NetFields.AddFields(this.capacity, this.inventory);
        }

        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            return false;
        }

        public override bool canBeTrashed()
        {
            return this.inventory.Count == 0;
        }

        public override bool performUseAction(GameLocation location)
        {
            return this.openInventoryMenu();
        }

        /// <summary>
        /// Opens the inventory of the item bag to display it's contents.
        /// </summary>
        /// <returns></returns>
        public virtual bool openInventoryMenu()
        {
            if (Game1.activeClickableMenu == null)
            {
                Game1.activeClickableMenu = new InventoryDisplayMenu(null, null, this.inventory, this.Capacity, this);
                return false;
            }
            return false;
        }

        public override Item getOne()
        {
            return new StorageBag(this.basicItemInformation.Copy(), this.Capacity);
        }

        public override void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location)
        {
            //Don't draw placement bounds.
        }
    }
}
