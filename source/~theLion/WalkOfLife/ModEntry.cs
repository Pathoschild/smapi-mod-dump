/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

/// Walk Of Life .NET 4.52
using System;
using StardewModdingAPI;
using TheLion.Stardew.Professions.Framework;
using TheLion.Stardew.Professions.Framework.AssetEditors;
using TheLion.Stardew.Professions.Framework.AssetLoaders;

namespace TheLion.Stardew.Professions
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		internal static ModData Data { get; set; }
		internal static ModConfig Config { get; set; }
		internal static EventSubscriber Subscriber { get; private set; }
		internal static SoundEffectLoader SoundFX { get; set; }

		internal static IModHelper ModHelper { get; private set; }
		internal static IManifest Manifest { get; private set; }
		internal static Action<string, LogLevel> Log { get; private set; }

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			// store references to mod helper and metadata
			ModHelper = helper;
			Manifest = ModManifest;
			Log = Monitor.Log;

			// get configs and mod data
			Config = helper.ReadConfig<ModConfig>();
			Data = new(Manifest.UniqueID);

			// apply harmony patches
			new HarmonyPatcher(Manifest.UniqueID).ApplyAll();

			// start event subscriber
			Subscriber = new();

			// get mod assets
			helper.Content.AssetEditors.Add(new IconEditor()); // sprite assets
			SoundFX = new(helper.DirectoryPath); // sound assets

			// add debug commands
			ConsoleCommands.Register();
		}
	}
}