/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/BuildableGingerIslandFarm
**
*************************************************/

using StardewModdingAPI;
using BuildableGingerIslandFarm.Handlers;
using BuildableGingerIslandFarm.Utilities;

namespace BuildableGingerIslandFarm
{
	/// <summary>The mod entry point.</summary>
	internal sealed class ModEntry : Mod
	{
		// Shared static helpers
		internal static new IModHelper	Helper { get; private set; }
		internal static new IMonitor	Monitor { get; private set; }
		internal static new IManifest	ModManifest { get; private set; }

		public static ModConfig	Config;

		public override void Entry(IModHelper helper)
		{
			Helper = base.Helper;
			Monitor = base.Monitor;
			ModManifest = base.ModManifest;

			// Subscribe to events
			Helper.Events.GameLoop.GameLaunched += GameLaunchedHandler.Apply;
			Helper.Events.GameLoop.DayStarted += DayStartedHandler.Apply;
			Helper.Events.Content.AssetRequested += AssetRequestedHandler.Apply;
		}
	}
}
