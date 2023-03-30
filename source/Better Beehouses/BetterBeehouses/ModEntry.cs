/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/BetterBeehouses
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using BetterBeehouses.integration;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace BetterBeehouses
{
	public class ModEntry : Mod
	{
		internal static ITranslationHelper i18n;
		internal static IMonitor monitor;
		internal static IModHelper helper;
		internal static Harmony harmony;
		internal static string ModID;
		internal static Config config;
		internal static API api;
		internal static IAeroCoreAPI AeroCore;
		internal static Texture2D BeeTex => beeTex ??= helper.GameContent.Load<Texture2D>("Mods/BetterBeehouses/Bees");
		private static Texture2D beeTex;

		public override void Entry(IModHelper helper)
		{
			Monitor.Log(helper.Translation.Get("general.startup"), LogLevel.Debug);

			monitor = Monitor;
			ModEntry.helper = Helper;
			harmony = new(ModManifest.UniqueID);
			ModID = ModManifest.UniqueID;
			config = helper.ReadConfig<Config>();
			api = new();
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.Content.AssetRequested += AssetRequested;
			helper.Events.GameLoop.DayStarted += (s, e) => CJBPatch.ReloadFruits();
			i18n = helper.Translation;
		}
		private void OnGameLaunched(object sender, GameLaunchedEventArgs ev)
		{
			UtilityPatch.Init();
			monitor.Log(helper.Translation.Get("general.patchedModsWarning"), LogLevel.Trace);
			if (helper.ModRegistry.IsLoaded("tlitookilakin.AeroCore") &&
				helper.ModRegistry.Get("tlitookilakin.AeroCore").Manifest.Version.IsNewerThan("0.9.4"))
				AeroCore = helper.ModRegistry.GetApi<IAeroCoreAPI>("tlitookilakin.AeroCore");
			if (helper.ModRegistry.IsLoaded("Pathoschild.Automate") && !config.PatchAutomate)
				monitor.Log(i18n.Get("general.automatePatchDisabled"), LogLevel.Info);
			if (helper.ModRegistry.IsLoaded("Digus.ProducerFrameworkMod") && !config.PatchPFM)
				monitor.Log(i18n.Get("general.pfmPatchDisabled"), LogLevel.Info);
			BeeManager.Init();
			harmony.PatchAll();
			config.Patch();
			WildFlowers.Setup();
			config.RegisterModConfigMenu(ModManifest);
		}
		public override object GetApi()
			=> api;
		private void AssetRequested(object _, AssetRequestedEventArgs ev)
		{
			if (config.Particles && AeroCore is null && ev.NameWithoutLocale.IsEquivalentTo("Mods/aedenthorn.ParticleEffects/dict"))
				ev.Edit(data => Utils.AddDictionaryEntry(data, "tlitookilakin.BetterBeehouses.Bees", "beeParticle.json"));
			else if (ev.NameWithoutLocale.IsEquivalentTo("Data/ObjectContextTags"))
				ev.Edit(AddTags);
			else if (ev.NameWithoutLocale.IsEquivalentTo("Mods/BetterBeehouses/Bees"))
				ev.LoadFromModFile<Texture2D>("assets/bees.png", AssetLoadPriority.Medium);
		}
		private void AssetInvalidated(object _, AssetsInvalidatedEventArgs ev)
		{
			foreach (var name in ev.NamesWithoutLocale)
				if (name.IsEquivalentTo("Mods/BetterBeehouses/Bees"))
					beeTex = null;
		}
		private static void AddTags(IAssetData asset)
		{
			var data = asset.AsDictionary<string, string>().Data;
			data["Daffodil"] += ", honey_source";
			data["Dandelion"] += ", honey_source";
		}
	}
}
