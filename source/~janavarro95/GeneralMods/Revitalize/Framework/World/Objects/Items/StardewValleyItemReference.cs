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
using Microsoft.Xna.Framework;
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Items
{
    [XmlType("Mods_Revitalize.Framework.World.Objects.Items.StardewValleyItemReference")]
    public class StardewValleyItemReference : ItemReference
    {

        public readonly NetEnum<Enums.SDVObject> objectId = new NetEnum<Enums.SDVObject>();

        public StardewValleyItemReference()
        {
            this.objectId.Value = Enums.SDVObject.NULL;
        }

        public StardewValleyItemReference(Enums.SDVObject objectId, int StackSize = 1) : base(StackSize)
        {
            this.objectId.Value = objectId;
        }

        public override Item getItem()
        {
            return this.getItem(this.stackSize.Value);
        }

        public override Item getItem(int StackSize = 1)
        {
            if (this.objectId.Value != Enums.SDVObject.NULL)
                return new StardewValley.Object(Vector2.Zero, (int)this.objectId.Value, StackSize);
            return null;
        }

        public override List<INetSerializable> getNetFields()
        {
            List<INetSerializable> netFields = base.getNetFields();
            netFields.Add(this.objectId);
            return netFields;
        }

        public override ItemReference readItemReference(BinaryReader reader)
        {
            base.readItemReference(reader);
            this.objectId.Value = reader.ReadEnum<Enums.SDVObject>();
            return this;
        }

        public override void writeItemReference(BinaryWriter writer)
        {
            base.writeItemReference(writer);
            writer.WriteEnum(this.objectId.Value);
        }
    }
}
