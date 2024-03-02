/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using WarpNetwork.models;

namespace WarpNetwork.framework
{
	class DataPatcher
	{
		private static readonly string[] DefaultDests = 
			{ "farm", "mountain", "beach", "desert", "island" };

		private static readonly string[] knownIcons =
			{ "DEFAULT", "farm", "mountain", "island", "desert", "beach", "RETURN" };

		private static readonly Func<LegacyWarpLocation, WarpLocation> convert_destination =
			typeof(LegacyWarpLocation).GetMethod(nameof(LegacyWarpLocation.Convert))
			.CreateDelegate<Func<LegacyWarpLocation, WarpLocation>>();

		// TODO replace statue edits with location edits

		internal static void Init()
		{
			ModEntry.helper.Events.Content.AssetRequested += AssetRequested;
		}

		internal static void AssetRequested(object _, AssetRequestedEventArgs ev)
		{
			if (ev.NameWithoutLocale.IsDirectlyUnderPath(ModEntry.AssetPath)) 
			{
				var name = ev.NameWithoutLocale.ToString().WithoutPath(ModEntry.AssetPath);
				switch (name)
				{
					case "Totems":
						ev.LoadFromModFile<Dictionary<string, WarpItem>>("assets/Totems.json", AssetLoadPriority.Medium);
						ev.Edit(static (a) => PortData(a.Data as Dictionary<string, WarpItem>, "WarpItems")); 
						break;
					case "PlacedTotems":
						ev.LoadFromModFile<Dictionary<string, WarpItem>>("assets/PlacedTotems.json", AssetLoadPriority.Medium);
						ev.Edit(static (a) => PortData(a.Data as Dictionary<string, WarpItem>, "Objects")); 
						break;
					case "Destinations":
						ev.LoadFromModFile<Dictionary<string, WarpLocation>>("assets/Destinations.json", AssetLoadPriority.Medium);
						ev.Edit(static (a) => PortData(a.Data as Dictionary<string, WarpLocation>, "Destinations", convert_destination));
						break;

				}
			} else if (ev.NameWithoutLocale.IsDirectlyUnderPath(ModEntry.LegacyAssetPath))
			{
				var name = ev.NameWithoutLocale.ToString().WithoutPath(ModEntry.LegacyAssetPath);
				switch (name)
				{
					case "Objects":
					case "WarpItems":
						ev.LoadFrom(static () => new Dictionary<string, WarpItem>(), AssetLoadPriority.Low);
						break;
					case "Destinations":
						ev.LoadFrom(static () => new Dictionary<string, LegacyWarpLocation>(), AssetLoadPriority.Low);
						break;
				}
			} else if (ev.NameWithoutLocale.IsDirectlyUnderPath(ModEntry.AssetPath + "/Icons"))
			{
				var name = ev.NameWithoutLocale.ToString().WithoutPath(ModEntry.AssetPath + "/Icons");
				if (knownIcons.Contains(name))
					ev.LoadFromModFile<Texture2D>($"assets/icons/{name}.png", AssetLoadPriority.Low);
			}
		}

		private static void PortData<T>(Dictionary<string, T> data, string oldName)
		{
			var legacy = ModEntry.helper.GameContent.Load<Dictionary<string, T>>(ModEntry.LegacyAssetPath + '/' + oldName);
			foreach ((var key, var val) in legacy)
				data.TryAdd(key, val);
		}

		private static void PortData<TI, TO>(Dictionary<string, TO> data, string oldName, Func<TI, TO> converter)
		{
			var legacy = ModEntry.helper.GameContent.Load<Dictionary<string, TI>>(ModEntry.LegacyAssetPath + '/' + oldName);
			foreach ((var key, var val) in legacy)
				data.TryAdd(key, converter(val));
		}
	}
}
