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
    public class NetStringStringDictionary : NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>
    {
        public NetStringStringDictionary() { }
        public NetStringStringDictionary(IEnumerable<KeyValuePair<string, string>> dict)
        {
            CopyFrom(dict);
        }

        protected override string getFieldTargetValue(NetString field)
        {
            return field.Value;
        }

        protected override string getFieldValue(NetString field)
        {
            return field.Value;
        }

        protected override string ReadKey(BinaryReader reader)
        {
            return reader.ReadString();
        }

        protected override void setFieldValue(NetString field, string key, string value)
        {
            field.Value = value;
        }

        protected override void WriteKey(BinaryWriter writer, string key)
        {
            writer.Write(key);
        }
    }
}
