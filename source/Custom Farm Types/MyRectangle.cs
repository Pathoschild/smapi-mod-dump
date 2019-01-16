using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace CustomFarmTypes
{
    [JsonConverter(typeof(MyRectangleConverter))]
    public class MyRectangle
    {
        public int x;
        public int y;
        public int w;
        public int h;

        public MyRectangle() { }
        public MyRectangle(int xx, int yy, int ww, int hh)
        {
            x = xx;
            y = yy;
            w = ww;
            h = hh;
        }

        public bool contains(int xx, int yy )
        {
            return x <= xx && x + w >= xx && y <= yy && y + h >= yy;
        }
    }

    // http://stackoverflow.com/a/21359362
    public class MyRectangleConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var rectangle = (MyRectangle)value;

            var x = rectangle.x;
            var y = rectangle.y;
            var width = rectangle.w;
            var height = rectangle.h;

            var o = JObject.FromObject(new { x, y, width, height });

            o.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var o = JObject.Load(reader);

            var x = GetTokenValue(o, "x") ?? 0;
            var y = GetTokenValue(o, "y") ?? 0;
            var width = GetTokenValue(o, "width") ?? 0;
            var height = GetTokenValue(o, "height") ?? 0;

            return new MyRectangle(x, y, width, height);
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        private static int? GetTokenValue(JObject o, string tokenName)
        {
            JToken t;
            return o.TryGetValue(tokenName, StringComparison.InvariantCultureIgnoreCase, out t) ? (int)t : (int?)null;
        }
    }
}
