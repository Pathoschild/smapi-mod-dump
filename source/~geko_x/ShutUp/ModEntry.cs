/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/geko_x/stardew-mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using HarmonyLib;

namespace ShutUp {
	class ModEntry : Mod {

		public static Mod INSTANCE;
		public static IModHelper modhelper;
		public static ITranslationHelper i18n;

		public static ModConfig config;
		public static IMonitor monitor;


		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper) {
			INSTANCE = this;
			modhelper = helper;
			i18n = helper.Translation;
			monitor = Monitor;

			Monitor.Log("Mod Entry", LogLevel.Trace);

			Monitor.Log("Reading config", LogLevel.Debug);
			ModEntry.config = helper.ReadConfig<ModConfig>();

			var harmony = new Harmony(this.ModManifest.UniqueID);

			// public static bool playSound(string cueName, int? pitch = null)
			//harmony.Patch(
			//	original: AccessTools.Method(typeof(StardewValley.Game1), nameof(StardewValley.Game1.playSound)),
			//	prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.playSound_prePatch_GekoX_ShutUp))
			//);

			Monitor.Log("Patching StardewValley.Audio.SoundsHelper.PlayLocal", LogLevel.Debug);
			harmony.Patch(
				original: AccessTools.Method(typeof(StardewValley.Audio.SoundsHelper), nameof(StardewValley.Audio.SoundsHelper.PlayLocal)),
				prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.playLocal_PrePatch_GekoX_ShutUp))
			);

			Monitor.Log("Done", LogLevel.Debug);
		}

	}

	class ModConfig {
		public string[] sounds { get; set; } = new[] { "" };
		public bool showDebugSpam = false;
	}
}
