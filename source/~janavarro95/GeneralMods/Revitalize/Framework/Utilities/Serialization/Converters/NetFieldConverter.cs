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
using Newtonsoft.Json;

namespace Revitalize.Framework.Utilities.Serialization.Converters
{

    public class NetFieldConverter:JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue("NetFields");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
        
            string str = (string)reader.Value;
            //string str=jsonObject.ToObject<string>(serializer);

            return new Netcode.NetFields();
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Netcode.NetFields);
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}
