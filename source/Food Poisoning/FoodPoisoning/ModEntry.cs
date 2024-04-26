/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/seferoni/FoodPoisoning
**
*************************************************/

#region global using directives

global using System;
global using System.Collections.Generic;
global using SharedLibrary.Interfaces.GMCM;
global using SharedLibrary.Integrations.GMCM;
global using StardewValley;
global using StardewModdingAPI;

#endregion

namespace FoodPoisoning;

#region using directives

using HarmonyLib;
using FoodPoisoning.Interfaces;
using StardewModdingAPI.Events;

#endregion

internal sealed class ModEntry : Mod
{
	internal static ModConfig Config { get; set; } = null!;
	public override void Entry(IModHelper helper)
	{
		Config = Helper.ReadConfig<ModConfig>();
		InitialiseHarmony();

		// Event subscriptions.
		helper.Events.GameLoop.GameLaunched += GameLaunched;
	}

	private void GameLaunched(object? sender, GameLaunchedEventArgs e)
	{
		SetupConfig();
	}

	private void InitialiseHarmony()
	{
		Harmony harmonyInstance = new(ModManifest.UniqueID);
		HarmonyPatcher.InitialiseMonitor(Monitor);

		harmonyInstance.Patch
		(
			original: AccessTools.Method(typeof(Farmer), nameof(Farmer.doneEating)),
			postfix: new HarmonyMethod(typeof(HarmonyPatcher), nameof(HarmonyPatcher.DoneEating_PostFix))
		);
	}

	private void SetupConfig()
	{
		var GMCMInterface = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

		if (GMCMInterface is null)
		{
			return;
		}

		GMCMHelper.Initialise(GMCMInterface, Helper, ModManifest);
		GMCMHelper.Build(Config);
	}
}