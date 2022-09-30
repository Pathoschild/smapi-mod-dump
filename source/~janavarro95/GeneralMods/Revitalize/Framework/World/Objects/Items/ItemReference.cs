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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Netcode;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Items
{
    [XmlType("Mods_Revitalize.Framework.World.Objects.Items.ItemReference")]
    public class ItemReference
    {
        /// <summary>
        /// The default stack size for getting the item when using <see cref="getItem()"/>
        /// </summary>
        public readonly NetInt stackSize = new NetInt();

        public ItemReference() : this(1)
        {

        }

        public ItemReference(int StackSize = 1)
        {
            this.stackSize.Value = StackSize;
        }

        public virtual Item getItem(int StackSize = 1)
        {
            return null;
        }

        public virtual Item getItem()
        {
            return null;
        }

        public virtual List<INetSerializable> getNetFields()
        {
            return new List<INetSerializable>()
            {
                this.stackSize
            };
        }

        public virtual ItemReference readItemReference(BinaryReader reader)
        {
            this.stackSize.Value = reader.ReadInt32();
            return this;
        }

        public virtual void writeItemReference(BinaryWriter writer)
        {
            writer.Write(this.stackSize.Value);
        }
    }
}
