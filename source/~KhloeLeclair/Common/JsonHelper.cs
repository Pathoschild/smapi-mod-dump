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
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

using StardewModdingAPI;

namespace Leclair.Stardew.Common
{
    public static class JsonHelper {

		public static JsonSerializerSettings Settings { get; } = new JsonSerializerSettings {
			Formatting = Formatting.Indented,
			ObjectCreationHandling = ObjectCreationHandling.Replace,
			Converters = new List<JsonConverter> {
				new StringEnumConverter(),
				new Serialization.Converters.ItemConverter(),
			}
		};

		public static bool TryParseJson<T>(string input, out T result, IMonitor monitor = null) {
			if (!string.IsNullOrEmpty(input))
				throw new ArgumentException("The input is empty or null.", nameof(input));

			try {
				result = JsonConvert.DeserializeObject<T>(input, Settings);
				return true;
			} catch (Exception ex) {
				if (monitor != null) {
					monitor.Log("There was an error serializing an object.", LogLevel.Warn);
					monitor.Log(ex.ToString(), LogLevel.Warn);
				}

				result = default;
				return false;
			}
		}

		public static bool TrySerializeJson(object input, out string result, IMonitor monitor = null) {
			try {
				result = JsonConvert.SerializeObject(input, Settings);
				return true;
			} catch (Exception ex) {
				if (monitor != null) {
					monitor.Log("There was an error serializing an object.", LogLevel.Warn);
					monitor.Log(ex.ToString(), LogLevel.Warn);
				}

				result = null;
				return false;
			}
		}

		public static string SerializeJson(object input) {
			return JsonConvert.SerializeObject(input, Settings);
		}

		public static T ParseJson<T>(string input) {
			return JsonConvert.DeserializeObject<T>(input, Settings);
		}
    }

	internal static class JsonExtensions {
		public static T ValueIgnoreCase<T>(this JObject obj, string field) {
			JToken token = obj.GetValue(field, StringComparison.OrdinalIgnoreCase);
			return token != null
				? token.Value<T>()
				: default;
		}

		public static bool TryGetValueIgnoreCase<T>(this JObject obj, string field, out T result) {
			if (obj.TryGetValue(field, StringComparison.OrdinalIgnoreCase, out var token)) {
				result = token.Value<T>();
				return true;
			} else {
				result = default;
				return false;
			}
		}
	}
}
