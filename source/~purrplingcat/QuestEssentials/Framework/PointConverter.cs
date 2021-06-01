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
    internal class PointConverter : JsonConverter<Point>
    {
        public override bool CanWrite => false;

        public override Point ReadJson(JsonReader reader, Type objectType, Point existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
                throw new JsonException($"Can't parse {typeof(Point).Name} from {reader.TokenType}");

            string str = (string)reader.Value;
            string[] parts = str.Split(',');
            if (parts.Length != 2)
                throw new JsonException($"Can't parse {nameof(Point)} from invalid value `{str}`");

            int x = Convert.ToInt32(parts[0]);
            int y = Convert.ToInt32(parts[1]);
            return new Point(x, y);
        }

        public override void WriteJson(JsonWriter writer, Point value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
