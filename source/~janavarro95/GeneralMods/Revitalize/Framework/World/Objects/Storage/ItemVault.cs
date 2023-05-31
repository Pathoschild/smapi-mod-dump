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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Netcode;
using Newtonsoft.Json;
using Omegasis.Revitalize.Framework.Menus.Items;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Storage
{
    /// <summary>
    /// A class to handle additional, condensed storage containers.
    /// </summary>
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Storage.ItemVault")]
    public class ItemVault : CustomObject
    {

        public readonly NetInt capacity = new NetInt();

        [XmlIgnore]
        public int Capacity { get => this.capacity.Value; set => this.capacity.Value = value; }

        public readonly NetObjectList<Item> inventory = new NetObjectList<Item>();

        public ItemVault() { }

        public ItemVault(BasicItemInformation basicItemInformation, int Capacity) : base(basicItemInformation)
        {

            this.Capacity = Capacity;
        }


        protected override void initializeNetFieldsPostConstructor()
        {
            base.initializeNetFieldsPostConstructor();
            this.NetFields.AddFields(this.capacity, this.inventory);
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (Game1.activeClickableMenu == null)
            {
                Game1.activeClickableMenu = new InventoryDisplayMenu(null,null ,this.inventory, this.Capacity, this);
                return true;
            }
            return false;
        }

        public override Item getOne()
        {
            return new ItemVault(this.basicItemInformation.Copy(), this.Capacity);
        }

        public override bool canBeRemoved(Farmer who = null)
        {
            //Prevent the player from being able to pick up item vaults if the vault is storing internal contents.
            if (this.inventory.Count > 0)
            {
                return false;
            }
            return base.canBeRemoved(who);
        }
    }
}
