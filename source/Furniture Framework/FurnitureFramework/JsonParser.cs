/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/FurnitureFramework
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;

namespace FurnitureFramework
{
	static class JsonParser
	{
		public static bool try_parse<T>(JToken? token, ref T result)
		{
			if (token is null || token.Type == JTokenType.Null)
				return false;

			if (result is Rectangle)
			{
				try_parse(token, out Rectangle rect);
				result = (T)(object)rect;
				return true;
			}
			
			if (token is JArray or JObject) return false;
			
			T? n_result = token.ToObject<T?>();
			if (n_result is null) return false;

			result = n_result;
			return true;
		}

		public static T parse<T>(JToken? token, T def)
		{
			if (token is null || token.Type == JTokenType.Null)
				return def;
			
			T? n_result = token.ToObject<T?>();
			if (n_result is null) return def;

			return n_result;
		}

		private static bool is_num(JToken token)
		{
			return 
				token.Type == JTokenType.Float ||
				token.Type == JTokenType.Integer;
		}

		public static bool try_parse(JToken? token, ref Vector2 result)
		{
			if (token is not JObject obj) return false;
			
			JToken? X_token = obj.GetValue("X");
			if (X_token == null || !is_num(X_token)) return false;
			result.X = (float)X_token;

			JToken? Y_token = obj.GetValue("Y");
			if (Y_token == null || !is_num(Y_token)) return false;
			result.Y = (float)Y_token;

			return true;
		}

		public static bool try_parse(JToken? token, ref Point result)
		{
			if (token is not JObject obj) return false;
			
			JToken? X_token = obj.GetValue("X");
			if (X_token == null || X_token.Type != JTokenType.Integer) return false;
			result.X = (int)X_token;

			JToken? Y_token = obj.GetValue("Y");
			if (Y_token == null || Y_token.Type != JTokenType.Integer) return false;
			result.Y = (int)Y_token;

			return true;
		}

		public static bool try_parse(JToken? token, out Rectangle result)
		{
			result = Rectangle.Empty;
			if (token is not JObject obj) return false;
			
			JToken? X_token = obj.GetValue("X");
			if (X_token == null || X_token.Type != JTokenType.Integer) return false;
			result.X = (int)X_token;

			JToken? Y_token = obj.GetValue("Y");
			if (Y_token == null || Y_token.Type != JTokenType.Integer) return false;
			result.Y = (int)Y_token;
			
			JToken? W_token = obj.GetValue("Width");
			if (W_token == null || W_token.Type != JTokenType.Integer) return false;
			result.Width = (int)W_token;

			JToken? H_token = obj.GetValue("Height");
			if (H_token == null || H_token.Type != JTokenType.Integer) return false;
			result.Height = (int)H_token;

			return true;
		}

		public static bool try_parse(JToken? token, ref List<string> list)
		{
			if (token is JArray array)
			{
				foreach (JToken sub_token in array.Children())
				{
					if (sub_token.Type != JTokenType.String) continue;
					list.Add(sub_token.ToString());
				}

				return true;
			}

			return false;
		}

		public static bool try_parse(JToken? token, List<string> rot_names, ref List<Rectangle?> result)
		{
			result.Clear();
			if (rot_names.Count == 0)
			{
				if(!try_parse(token, out Rectangle rect)) return false;
				result.Add(rect);
				return true;
			}
			else if (token is JObject rect_dict)
			{
				foreach (string key in rot_names)
				{
					JToken? rect_token = rect_dict.GetValue(key);
					if (try_parse(rect_token, out Rectangle rect))
						result.Add(rect);
					else result.Add(null);
				}
				return true;
			}
			return false;
		}

		public static bool try_parse(JToken? token, List<string> rot_names, ref List<int?> result)
		{
			result.Clear();
			int i = 0;

			if (rot_names.Count == 0)
			{
				if(!try_parse(token, ref i)) return false;
				result.Add(i);
				return true;
			}
			else if (token is JObject rect_dict)
			{
				foreach (string key in rot_names)
				{
					JToken? int_token = rect_dict.GetValue(key);
					if (try_parse(int_token, ref i))
						result.Add(i);
					else result.Add(null);
				}
				return true;
			}
			return false;
		}

		public static bool try_rm_null<T>(List<T?> list, ref List<T> result) where T: struct
		{
			result.Clear();
			foreach (T? val in list)
			{
				if (val is null) return false;
				result.Add(val.Value);
			}
			return true;
		}
	}
}