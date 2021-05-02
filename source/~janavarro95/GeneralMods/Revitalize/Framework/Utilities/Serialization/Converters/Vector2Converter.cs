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
using Newtonsoft.Json;

namespace Revitalize.Framework.Utilities.Serialization.Converters
{
    public class Vector2Converter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var rect = (Vector2)value;
            writer.WriteValue(rect.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {

            string str = (string)reader.Value;
            //string str=jsonObject.ToObject<string>(serializer);


            //string str = reader.ReadAsString();
            str = str.Replace("{", "");
            str = str.Replace("}", "");
            str = str.Replace(",", "");

            string[] values = str.Split(' ');
            double x = Convert.ToDouble(values[0].Split(':')[1]);
            double y = Convert.ToDouble(values[1].Split(':')[1]);


            return new Vector2((float)x, (float)y);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2);
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}
