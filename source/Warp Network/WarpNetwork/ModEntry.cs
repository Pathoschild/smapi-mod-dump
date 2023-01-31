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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using WarpNetwork.api;
using WarpNetwork.models;

namespace WarpNetwork
{
	class ModEntry : Mod
	{
		//const
		public static readonly string pathLocData = PathUtilities.NormalizeAssetName("Data/WarpNetwork/Destinations");
		public static readonly string pathItemData = PathUtilities.NormalizeAssetName("Data/WarpNetwork/WarpItems");
		public static readonly string pathIcons = PathUtilities.NormalizeAssetName("Data/WarpNetwork/Icons");
		public static readonly string pathObjectData = PathUtilities.NormalizeAssetName("Data/WarpNetwork/Objects");
		internal static readonly HashSet<string> knownIcons = new(new[] {"DEFAULT", "farm", "mountain", "island", "desert", "beach", "RETURN"});

		//main
		internal static Config config;
		internal static IModHelper helper;
		internal static IMonitor monitor;
		internal static ITranslationHelper i18n;
		internal static AeroCore.API.IAeroCoreAPI AeroAPI;
		public static API api = new();

		public override void Entry(IModHelper helper)
		{
			config = helper.ReadConfig<Config>();
			ModEntry.helper = helper;
			monitor = Monitor;
			i18n = helper.Translation;

			helper.Events.Content.AssetRequested += LoadAssets;
			helper.Events.GameLoop.GameLaunched += GameLaunched;
			LocalizedContentManager.OnLanguageChange += (c) => helper.GameContent.InvalidateCache(pathLocData);
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void GameLaunched(object sender, GameLaunchedEventArgs ev)
		{
			AeroAPI = helper.ModRegistry.GetApi<AeroCore.API.IAeroCoreAPI>("tlitookilakin.AeroCore");
			AeroAPI.RegisterGMCMConfig(ModManifest, helper, config, 
				() => helper.GameContent.InvalidateCache(pathLocData)
			);
			AeroAPI.InitAll();
			CPIntegration.AddTokens(ModManifest);
		}
		public override object GetApi() => api;
		private void LoadAssets(object _, AssetRequestedEventArgs ev)
		{
			if (ev.NameWithoutLocale.IsEquivalentTo(pathObjectData))
				ev.LoadFromModFile<Dictionary<string, WarpItem>>("assets/WarpObjects.json", AssetLoadPriority.Low);
			else if (ev.NameWithoutLocale.IsEquivalentTo(pathLocData))
				ev.LoadFromModFile<Dictionary<string, WarpLocation>>("assets/Destinations.json", AssetLoadPriority.Medium);
			else if (ev.NameWithoutLocale.IsEquivalentTo(pathItemData))
				ev.LoadFromModFile<Dictionary<string, WarpItem>>("assets/WarpItems.json", AssetLoadPriority.Medium);
			else if (ev.NameWithoutLocale.StartsWith(pathIcons))
			{
				var name = ev.NameWithoutLocale.ToString().WithoutPath(pathIcons);
				if (knownIcons.Contains(name))
					ev.LoadFromModFile<Texture2D>($"assets/icons/{name}.png", AssetLoadPriority.Low);
				else
					ev.LoadFrom(() => helper.GameContent.Load<Texture2D>($"{pathIcons}/DEFAULT"), AssetLoadPriority.Low);
			}
		}
	}
}
