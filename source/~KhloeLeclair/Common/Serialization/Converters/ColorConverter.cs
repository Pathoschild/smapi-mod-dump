/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;

using Microsoft.Xna.Framework;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StardewValley;

namespace Leclair.Stardew.Common.Serialization.Converters {
	public class ColorConverter : JsonConverter {
		public override bool CanConvert(Type objectType) {
			// This will get easier in 1.6. For now we only care about SObjects.
			return typeof(Color?).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			string path = reader.Path;
			switch (reader.TokenType) {
				case JsonToken.Null:
					return null;
				case JsonToken.String:
					return ReadString(JToken.Load(reader).Value<string>());
				case JsonToken.StartObject:
					return ReadObject(JObject.Load(reader), path);
				default:
					throw new JsonReaderException($"Can't parse Color? from {reader.TokenType} node (path: {reader.Path}).");
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			if (value is not Color color) { 
				writer.WriteNull();
				return;
			}

			var jo = new JObject {
				{"R", color.R },
				{"G", color.G },
				{"B", color.B },
				{"A", color.A }
			};

			jo.WriteTo(writer);
		}

		private static Color? ReadString(string value) {
			return CommonHelper.ParseColor(value);
		}

		private static Color? ReadObject(JObject obj, string path) {
			try {
				if (!obj.TryGetValueIgnoreCase("R", out int R) ||
					!obj.TryGetValueIgnoreCase("G", out int G) ||
					!obj.TryGetValueIgnoreCase("B", out int B)
				)
					return null;

				if (obj.TryGetValueIgnoreCase("A", out int A))
					return new Color(R, G, B, A);

				return new Color(R, G, B);

			} catch(Exception ex) {
				throw new JsonReaderException($"Can't parse Color? from JSON object node (path: {path}).", ex);
			}
		}
	}
}
