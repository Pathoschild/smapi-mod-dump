/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ItemBags.Persistence
{
    [Serializable]
    [XmlRoot(ElementName = "KeyValuePair", Namespace = "")]
    public struct KeyValuePair<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
        public KeyValuePair(K Key, V Value)
        {
            this.Key = Key;
            this.Value = Value;
        }

        public override string ToString()
        {
            return string.Format("Key = {0}, Value = {1}", Key.ToString(), Value.ToString());
        }
    }
}
