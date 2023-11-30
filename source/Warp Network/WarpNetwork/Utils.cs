/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Globalization;
using WarpNetwork.api;
using WarpNetwork.models;
using xTile;
using DColor = System.Drawing.Color;

namespace WarpNetwork
{
	static class Utils
	{
		public static Dictionary<string, IWarpNetHandler> CustomLocs = new(StringComparer.OrdinalIgnoreCase);
		private static readonly string[] VanillaMapNames =
		{
			"Farm","Farm_Fishing","Farm_Foraging","Farm_Mining","Farm_Combat","Farm_FourCorners","Farm_Island"
		};
		private static readonly Dictionary<string, int> FarmTypeMap = new()
		{
			{ "farm", 0 },
			{ "farm_fishing", 1 },
			{ "farm_foraging", 2 },
			{ "farm_mining", 3 },
			{ "farm_combat", 4 },
			{ "farm_fourcorners", 5 },
			{ "farm_island", 6 }
		};
		public static Point GetActualFarmPoint(int default_x, int default_y)
			=> GetActualFarmPoint(Game1.getFarm().Map, default_x, default_y);
		public static Point GetActualFarmPoint(Map map, int default_x, int default_y, string filename = null)
		{
			int x = default_x;
			int y = default_y;
			switch (GetFarmType(filename))
			{
				//four corners
				case 5:
					x = 48;
					y = 39;
					break;
				//beach
				case 6:
					x = 82;
					y = 29;
					break;
			}
			return GetMapPropertyPosition(map, "WarpTotemEntry", x, y);
		}
		public static string GetFarmMapPath()
		{
			if (Game1.whichFarm < 0)
			{
				ModEntry.monitor.Log("Something is wrong! Game1.whichfarm does not contain a valid value!", LogLevel.Warn);
				return "";
			}
			if (Game1.whichFarm < 7)
				return VanillaMapNames[Game1.whichFarm];
			if (Game1.whichModFarm is null)
			{
				ModEntry.monitor.Log("Something is wrong! Custom farm indicated, but Game1.whichModFarm is null!", LogLevel.Warn);
				return "";
			}
			return Game1.whichModFarm.MapName;
		}
		public static int GetFarmType(string filename)
			=> (filename is null) ? Game1.whichFarm : FarmTypeMap.TryGetValue(filename, out var type) ? type : 0;
		public static Point GetMapPropertyPosition(Map map, string property, int default_x, int default_y)
		{
			if (!map.Properties.ContainsKey(property))
				return new Point(default_x, default_y);
			string prop = map.Properties[property];
			string[] args = prop.Split(' ');
			if (args.Length < 2)
				return new Point(default_x, default_y);
			return new Point(int.Parse(args[0]), int.Parse(args[1]));
		}
		public static Dictionary<string, WarpLocation> GetWarpLocations()
		{
			Dictionary<string, WarpLocation> data = ModEntry.helper.GameContent.Load<Dictionary<string, WarpLocation>>(ModEntry.pathLocData);
			Dictionary<string, WarpLocation> ret = new(data, StringComparer.OrdinalIgnoreCase);
			foreach ((string key, IWarpNetHandler value) in CustomLocs)
			{
				if (ret.ContainsKey(key))
				{
					ModEntry.monitor.Log("Overwriting destination '" + key + "' with custom handler", LogLevel.Debug);
					ret[key] = new CustomWarpLocation(value);
				}
				else
				{
					ret.Add(key, new CustomWarpLocation(value));
				}
			}
			return ret;
		}
		public static Dictionary<string, WarpItem> GetWarpItems()
			=> new(ModEntry.helper.GameContent.Load<Dictionary<string, WarpItem>>(ModEntry.pathItemData), StringComparer.OrdinalIgnoreCase);
		public static Dictionary<string, WarpItem> GetWarpObjects()
			=> new(ModEntry.helper.GameContent.Load<Dictionary<string, WarpItem>>(ModEntry.pathObjectData), StringComparer.OrdinalIgnoreCase);
		public static string WithoutPath(this string path, string prefix)
			=> PathUtilities.GetSegments(path, PathUtilities.GetSegments(prefix).Length + 1)[^1];
		internal static bool IsAnyObeliskBuilt(ICollection<WarpLocation> locs)
		{
			foreach (var loc in locs)
				if (loc.RequiredBuilding is not null && DataPatcher.buildingTypes.Contains(loc.RequiredBuilding.Collapse()))
					return true;
			return false;
		}
		public static bool IsAccessible(this IDictionary<string, WarpLocation> dict, string id)
			=> (!id.Equals("farm", StringComparison.OrdinalIgnoreCase) || 
			ModEntry.config.FarmWarpEnabled != WarpEnabled.AfterObelisk || 
			IsAnyObeliskBuilt(dict.Values)) && 
			(dict[id]?.IsAccessible() ?? false);

		public static IEnumerable<Building> GetAllBuildings()
		{
			foreach (var loc in Game1.locations)
				for (int i = 0; i < loc.buildings.Count; i++)
					yield return loc.buildings[i];
		}
		
		public static string Collapse(this string source)
		{
			var src = source.AsSpan();
			var res = new char[src.Length];
			var rsp = res.AsSpan();

			int l = 0;
			int n = 0;
			for(int i = 0; i < src.Length; i++)
			{
				if (src[i] is ' ')
				{
					if (l != i)
					{
						src[l..i].CopyTo(rsp[n..]);
						n += i - l;
					}
					l = i + 1;
				}
			}
			if (l != src.Length)
			{
				src[l..].CopyTo(rsp[n..]);
				n += src.Length - l;
			}
			return new(res[..n]);
		}
		public static TemporaryAnimatedSprite WithItem(this TemporaryAnimatedSprite sprite, Item item)
		{
			sprite.CopyAppearanceFromItemId(item.QualifiedItemId);
			return sprite;
		}
		public static bool TryParseColor(this string str, out Color color)
		{
			color = Color.Transparent;

			if (str is null || str.Length == 0)
				return false;

			DColor c = DColor.FromName(str);
			if (c.ToArgb() != 0)
			{
				color = new(c.R, c.G, c.B, c.A);
				return true;
			}

			ReadOnlySpan<char> s = str.AsSpan();
			if (s[0] == '#')
			{
				if (s.Length <= 3)
					return false;

				if (s.Length > 6)
				{
					if (int.TryParse(s[1..3], NumberStyles.HexNumber, null, out int r) &&
						int.TryParse(s[3..5], NumberStyles.HexNumber, null, out int g) &&
						int.TryParse(s[5..7], NumberStyles.HexNumber, null, out int b))
					{
						if (s.Length > 8 && int.TryParse(s[7..9], NumberStyles.HexNumber, null, out int a))
							color = new(r, g, b, a);
						else
							color = new(r, g, b);
						return true;
					}
				}
				else
				{
					if (int.TryParse($"{s[1]}{s[1]}", NumberStyles.HexNumber, null, out int r) &&
						int.TryParse($"{s[2]}{s[2]}", NumberStyles.HexNumber, null, out int g) &&
						int.TryParse($"{s[3]}{s[3]}", NumberStyles.HexNumber, null, out int b))
					{
						if (s.Length > 4 && int.TryParse($"{s[4]}{s[4]}", NumberStyles.HexNumber, null, out int a))
							color = new(r, g, b, a);
						else
							color = new(r, g, b);
						return true;
					}
				}
			}
			else
			{
				string[] vals = str.Split(',', StringSplitOptions.RemoveEmptyEntries);
				if (vals.Length > 2)
				{
					if (int.TryParse(vals[0], out int r) &&
						int.TryParse(vals[1], out int g) &&
						int.TryParse(vals[2], out int b))
					{
						if (vals.Length > 3 && int.TryParse(vals[3], out int a))
							color = new Color(r, g, b, a);
						else
							color = new Color(r, g, b);
						return true;
					}
				}
			}
			return false;
		}
		public static Point GetPropertyPosition(this GameLocation where, string which, Point fallback)
		{
			if (where is null || !where.TryGetMapProperty(which, out var p))
				return fallback;

			var split = p.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			if (split.Length is 0 or 1 || !int.TryParse(split[0], out int x) || !int.TryParse(split[1], out int y))
				return fallback;

			return new(x, y);
		}
		internal static void AddQuickBool(this IGMCMAPI api, object inst, IManifest manifest, string prop)
		{
			var p = inst.GetType().GetProperty(prop);
			var cfname = prop.Decap();
			api.AddBoolOption(manifest,
				p.GetGetMethod().CreateDelegate<Func<bool>>(inst),
				p.GetSetMethod().CreateDelegate<Action<bool>>(inst),
				() => ModEntry.i18n.Get($"config.{cfname}.name"),
				() => ModEntry.i18n.Get($"config.{cfname}.desc")
			);
		}
		internal static void AddQuickEnum<TE>(this IGMCMAPI api, object inst, IManifest manifest, string prop) where TE : Enum
		{
			var p = inst.GetType().GetProperty(prop);
			var cfname = prop.Decap();
			var tenum = typeof(TE);
			var tname = tenum.Name.Decap();
			api.AddTextOption(manifest,
				() => p.GetValue(inst).ToString(),
				(s) => p.SetValue(inst, (TE)Enum.Parse(tenum, s)),
				() => ModEntry.i18n.Get($"config.{cfname}.name"),
				() => ModEntry.i18n.Get($"config.{cfname}.desc"),
				Enum.GetNames(tenum),
				(s) => ModEntry.i18n.Get($"config.{tname}.{s}")
			);
		}
		internal static string Decap(this string src)
			=> src.Length > 0 ? char.ToLower(src[0]) + src[1..] : string.Empty;
	}
}
