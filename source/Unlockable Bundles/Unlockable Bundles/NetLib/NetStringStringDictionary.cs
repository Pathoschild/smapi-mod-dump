/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/unlockable-bundles
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netcode;
using StardewValley;
using StardewValley.Network;

namespace Unlockable_Bundles.NetLib
{
    public class NetStringIntDictionary : NetDictionary<string, int, NetInt, SerializableDictionary<string, int>, NetStringDictionary<int, NetInt>>
    {
        public NetStringIntDictionary() { }
        public NetStringIntDictionary(IEnumerable<KeyValuePair<string, int>> dict)
        {
            CopyFrom(dict);
        }

        protected override int getFieldTargetValue(NetInt field)
        {
            return field.Value;
        }

        protected override int getFieldValue(NetInt field)
        {
            return field.Value;
        }

        protected override string ReadKey(BinaryReader reader)
        {
            return reader.ReadString();
        }

        protected override void setFieldValue(NetInt field, string key, int value)
        {
            field.Value = value;
        }

        protected override void WriteKey(BinaryWriter writer, string key)
        {
            writer.Write(key);
        }
    }
}
