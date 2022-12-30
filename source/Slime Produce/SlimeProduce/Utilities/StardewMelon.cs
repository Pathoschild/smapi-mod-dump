/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/andraemon/SlimeProduce
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SlimeProduce
{
    public static class StardewMelon
    {
		public static Dictionary<Color, List<int>> GetColoredObjects()
        {
			Dictionary<Color, List<int>> dict = new();

			foreach (Color color in ColorList.Values)
				dict[color] = new List<int>();

			foreach (KeyValuePair<int, string> objectInfo in Game1.objectInformation)
			{
				string name;
				string altName = "id_o_" + objectInfo.Key;

				try { name = objectInfo.Value.Split('/')[0]; } catch (Exception) { continue; }

				string[] contextTags;

				if (Game1.objectContextTags.TryGetValue(name, out string tags))
					contextTags = tags.Split(',', StringSplitOptions.TrimEntries);
				else if (Game1.objectContextTags.TryGetValue(altName, out string altTags))
					contextTags = altTags.Split(',', StringSplitOptions.TrimEntries);
				else continue;

				foreach (string tag in contextTags)
					if (tag.StartsWith("color_") && tag.Length > 6 && ColorList.TryGetValue(tag[6..], out Color color))
						dict[color].Add(objectInfo.Key);
			}

			return dict;
        }

		public static Multiplayer SMultiplayer = (Multiplayer)AccessTools.Field(typeof(Game1), "multiplayer").GetValue(null);

		public static Dictionary<Color, List<int>> ColoredObjects;

		public static Dictionary<string, Color> ColorList = new Dictionary<string, Color>()
		{
			{ "black", new Color(45, 45, 45) },
			{ "gray", Color.Gray },
			{ "white", Color.White },
			{ "pink", new Color(255, 163, 186) },
			{ "red", new Color(220, 0, 0) },
			{ "orange", new Color(255, 128, 0) },
			{ "yellow", new Color(255, 230, 0) },
			{ "green", new Color(10, 143, 0) },
			{ "blue", new Color(46, 85, 183) },
			{ "purple", new Color(115, 41, 181) },
			{ "brown", new Color(130, 73, 37) },
			{ "light_cyan", new Color(180, 255, 255) },
			{ "cyan", Color.Cyan },
			{ "aquamarine", Color.Aquamarine },
			{ "sea_green", Color.SeaGreen },
			{ "lime", Color.Lime },
			{ "yellow_green", Color.GreenYellow },
			{ "pale_violet_red", Color.PaleVioletRed },
			{ "salmon", new Color(255, 85, 95) },
			{ "jade", new Color(130, 158, 93) },
			{ "sand", Color.NavajoWhite },
			{ "poppyseed", new Color(82, 47, 153) },
			{ "dark_red", Color.DarkRed },
			{ "dark_orange", Color.DarkOrange },
			{ "dark_yellow", Color.DarkGoldenrod },
			{ "dark_green", Color.DarkGreen },
			{ "dark_blue", Color.DarkBlue },
			{ "dark_purple", Color.DarkViolet },
			{ "dark_pink", Color.DeepPink },
			{ "dark_cyan", Color.DarkCyan },
			{ "dark_gray", Color.DarkGray },
			{ "dark_brown", Color.SaddleBrown },
			{ "gold", Color.Gold },
			{ "copper", new Color(179, 85, 0) },
			{ "iron", new Color(197, 213, 224) },
			{ "iridium", new Color(105, 15, 255) }
		};
    }
}
