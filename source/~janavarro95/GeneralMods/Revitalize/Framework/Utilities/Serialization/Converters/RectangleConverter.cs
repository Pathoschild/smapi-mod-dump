using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Revitalize.Framework.Utilities.Serialization.Converters
{
    public class RectangleConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var rect = (Rectangle)value;
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
            str = str.Replace("X:", "");
            str = str.Replace("Y:", "");
            str = str.Replace("Width:", "");
            str = str.Replace("Height:", "");

            string[] values = str.Split(' ');
            int x = Convert.ToInt32(values[0]);
            int y = Convert.ToInt32(values[1]);
            int w = Convert.ToInt32(values[2]);
            int h = Convert.ToInt32(values[3]);


            return new Rectangle(x, y, w, h);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Rectangle);
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}
