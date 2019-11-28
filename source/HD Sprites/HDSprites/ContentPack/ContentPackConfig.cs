using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HDSprites.ContentPack
{
    public class WhenDictionary : Dictionary<string, string> { }

    public class ConfigSchemaConfig
    {
        public string AllowValues { get; set; }
        public string Default { get; set; }
    }

    public class DynamicTokensConfig
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public WhenDictionary When { get; set; } = new WhenDictionary();
    }

    public class ChangesConfig
    {
        public string Action { get; set; }
        public string Target { get; set; }
        public string FromFile { get; set; }
        [JsonConverter(typeof(RectangleConverter))]
        public Rectangle FromArea { get; set; }
        [JsonConverter(typeof(RectangleConverter))]
        public Rectangle ToArea { get; set; }
        public string Enabled { get; set; } = "True";
        public string Patchmode { get; set; } = "";
        public WhenDictionary When { get; set; } = new WhenDictionary();
    }
    
    public class ContentConfig
    {
        public Dictionary<string, ConfigSchemaConfig> ConfigSchema { get; set; } = new Dictionary<string, ConfigSchemaConfig>();
        public List<DynamicTokensConfig> DynamicTokens { get; set; } = new List<DynamicTokensConfig>();
        public List<ChangesConfig> Changes { get; set; } = new List<ChangesConfig>();
    }

    // Provides compatibility between Newtonsoft.Json and Microsoft.XNA.Framework.Rectangle 
    // Author: David Kennedy <https://stackoverflow.com/users/1015595/david-kennedy>
    // Date: Jan 26, 2014
    // Source: <https://stackoverflow.com/a/21359362>
    public class RectangleConverter : JsonConverter
    {     
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var rectangle = (Rectangle)value;

            var x = rectangle.X;
            var y = rectangle.Y;
            var width = rectangle.Width;
            var height = rectangle.Height;

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

            return new Rectangle(x, y, width, height);
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
