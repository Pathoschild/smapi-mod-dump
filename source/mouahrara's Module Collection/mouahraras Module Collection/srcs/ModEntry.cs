/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using mouahrarasModuleCollection.Modules;
using mouahrarasModuleCollection.Hooks;
using mouahrarasModuleCollection.Utilities;

namespace mouahrarasModuleCollection
{
	/// <summary>The mod entry point.</summary>
	internal sealed class ModEntry : Mod
	{
		// Shared static helpers
		internal static new IModHelper	Helper { get; private set; }
		internal static new IMonitor	Monitor { get; private set; }
		internal static new IManifest	ModManifest { get; private set; }
		internal static ModConfig		Config;

		public override void Entry(IModHelper helper)
		{
			Helper = base.Helper;
			Monitor = base.Monitor;
			ModManifest = base.ModManifest;

			Harmony harmony = new(ModManifest.UniqueID);

			// Apply modules
			ArcadeGamesModule.Apply(harmony);
			ClintsShopModule.Apply(harmony);
			CrystalariumsModule.Apply(harmony);
			FarmViewModule.Apply(harmony);
			FestivalsModule.Apply(harmony);
			MarniesShopModule.Apply(harmony);

			// Hook into the required events
			Helper.Events.GameLoop.GameLaunched += GameLaunchedHook.Apply;
		}
	}
}
