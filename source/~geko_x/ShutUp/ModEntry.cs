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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Harmony;

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

			var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

			//harmony.Patch(
			//	original: Type.GetType(nameof(StardewValley.Game1)).GetMethod(nameof(Game1.playSound), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance),
			//	prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.playSound_prePatch_GekoX_ShutUp))
			//);

			harmony.Patch(
				original: AccessTools.Method(typeof(StardewValley.Game1), nameof(StardewValley.Game1.playSound)),
				prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.playSound_prePatch_GekoX_ShutUp))
			);

			harmony.Patch(
				original: AccessTools.Method(typeof(StardewValley.Game1), nameof(StardewValley.Game1.playSoundPitched)),
				prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.playSoundPitched_prePatch_GekoX_ShutUp))
			);

		}

	}

	class ModConfig {
		public string[] sounds { get; set; } = new[] { "dog_pant", "dog_bark" };
	}
}
