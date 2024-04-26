/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/seferoni/HealthRework
**
*************************************************/

#region global using directives

global using System;
global using System.Collections.Generic;
global using SharedLibrary.Interfaces.GMCM;
global using SharedLibrary.Integrations.GMCM;
global using StardewModdingAPI;

#endregion

namespace HealthRework;

#region using directives

using HarmonyLib;
using StardewModdingAPI.Events;
using HealthRework.Common;
using HealthRework.Interfaces;
using SObject = StardewValley.Object;

#endregion

internal sealed class ModEntry : Mod
{
	internal static ModConfig Config { get; set; } = null!;

	public override void Entry(IModHelper helper)
	{
		Initialise();

		// Event subscriptions.
		helper.Events.GameLoop.GameLaunched += GameLaunched;
		helper.Events.GameLoop.DayEnding += DayEnding;
		helper.Events.GameLoop.Saving += Saving;
	}

	private void Saving(object? sender, SavingEventArgs e)
	{
		Utilities.RestoreHealth();
	}

	private void DayEnding(object? sender, DayEndingEventArgs e)
	{
		Utilities.SaveCurrentHealth();
	}
	private void GameLaunched(object? sender, GameLaunchedEventArgs e)
	{
		SetupConfig();
	}

	private void Initialise()
	{
		Config = Helper.ReadConfig<ModConfig>();
		InitialiseHarmony();
	}

	private void InitialiseHarmony()
	{
		Harmony harmonyInstance = new(ModManifest.UniqueID);
		HarmonyPatcher.InitialiseMonitor(Monitor);

		harmonyInstance.Patch
		(
			original: AccessTools.Method(typeof(SObject), nameof(SObject.healthRecoveredOnConsumption)),
			postfix: new HarmonyMethod(typeof(HarmonyPatcher), nameof(HarmonyPatcher.HealthRecoveredOnConsumption_PostFix))
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

