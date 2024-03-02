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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using WarpNetwork.api;
using WarpNetwork.models;
using DColor = System.Drawing.Color;

namespace WarpNetwork.framework
{
	static class Utils
	{
		public static Dictionary<string, IWarpNetAPI.IDestinationHandler> CustomLocs =
			new(StringComparer.OrdinalIgnoreCase) { {"_return", ReturnHandler.Instance } };

		public static Point GetTargetTile(GameLocation where, Point target = default)
		{
			if (!DataLoader.Locations(Game1.content).TryGetValue(where.Name, out var data) || !data.DefaultArrivalTile.HasValue)
				return default;

			var tile = data.DefaultArrivalTile.Value;
			if (target != default)
				tile = target;

			if (where is Farm)
				tile = GetFarmTile(where);

			if (where.TryGetMapPropertyAs("WarpNetworkEntry", out Point prop, false))
				tile = prop;

			return tile;
		}

		public static Point GetFarmTile(GameLocation farm)
		{
			Point tile = Game1.GetFarmTypeID() switch
			{
				// four corners
				"5" => new(48, 39),

				// beach
				"6" => new(82, 29),

				// everything else
				_ => new(48, 7),
			};

			if (farm.TryGetMapPropertyAs("WarpTotemEntry", out Point prop, false))
				tile = prop;

			return tile;
		}

		public static Dictionary<string, IWarpNetAPI.IDestinationHandler> GetWarpLocations()
		{
			Dictionary<string, IWarpNetAPI.IDestinationHandler> ret = new(
				ModEntry.helper.GameContent.Load<Dictionary<string, IWarpNetAPI.IDestinationHandler>>(ModEntry.AssetPath + "/Destinations"),
				StringComparer.OrdinalIgnoreCase
			);

			foreach ((var key, var val) in CustomLocs)
				ret[key] = val;

			return ret;
		}

		public static Dictionary<string, WarpItem> GetWarpItems()
			=> new(
				ModEntry.helper.GameContent.Load<Dictionary<string, WarpItem>>(ModEntry.AssetPath + "/Totems"), 
				StringComparer.OrdinalIgnoreCase
			);

		public static Dictionary<string, WarpItem> GetWarpObjects()
			=> new(
				ModEntry.helper.GameContent.Load<Dictionary<string, WarpItem>>(ModEntry.AssetPath + "/PlacedTotems"),
				StringComparer.OrdinalIgnoreCase
			);

		public static string WithoutPath(this string path, string prefix)
			=> PathUtilities.GetSegments(path, PathUtilities.GetSegments(prefix).Length + 1)[^1];

		public static bool IsAccessible(this IDictionary<string, IWarpNetAPI.IDestinationHandler> dict, string id, GameLocation where, Farmer who)
			=> ModEntry.config.OverrideEnabled switch
			{
				WarpEnabled.Never => false,
				WarpEnabled.Always => true,
				WarpEnabled.Default =>
					dict.TryGetValue(id, out var loc) &&
					loc.IsAccessible(where, who),
				_ => false
			};

		public static string ToLocalLocale(this IModHelper helper, string Locale)
		{
			var split = Locale.LastIndexOf('-');
			var broad = split < 0 ? null : Locale[..split];

			var fname = Path.Join(helper.DirectoryPath, "i18n", Locale + ".json");

			if (File.Exists(fname))
				return Locale;

			fname = Path.Join(helper.DirectoryPath, "i18n", broad + ".json");

			if (File.Exists(fname))
				return broad;

			return "default";
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
