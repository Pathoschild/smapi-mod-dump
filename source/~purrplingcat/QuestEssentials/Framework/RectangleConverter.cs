/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestEssentials.Framework
{
    class RectangleConverter : JsonConverter<Rectangle>
    {
        public override bool CanWrite => false;

        public override Rectangle ReadJson(JsonReader reader, Type objectType,  Rectangle existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
                throw new JsonException($"Can't parse {typeof(Rectangle).Name} from {reader.TokenType}");

            string str = (string)reader.Value;
            string[] parts = str.Split(',');
            if (parts.Length != 4)
                throw new JsonException($"Can't parse {nameof(Rectangle)} from invalid value `{str}`");

            int x = Convert.ToInt32(parts[0]);
            int y = Convert.ToInt32(parts[1]);
            int width = Convert.ToInt32(parts[2]);
            int height = Convert.ToInt32(parts[3]);

            return new Rectangle(x, y, width, height);
        }

        public override void WriteJson(JsonWriter writer, Rectangle value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
