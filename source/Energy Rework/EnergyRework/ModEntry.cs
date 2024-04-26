/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/seferoni/EnergyRework
**
*************************************************/

#region global using directives

global using System;
global using System.Collections.Generic;
global using SharedLibrary.Interfaces.GMCM;
global using SharedLibrary.Integrations.GMCM;
global using StardewModdingAPI;

#endregion

namespace EnergyRework;

#region using directives

using StardewModdingAPI.Events;
using EnergyRework.Common;

#endregion

internal sealed class ModEntry : Mod
{
	internal static ModConfig Config { get; set; } = null!;
	public override void Entry(IModHelper helper)
	{
		Config = Helper.ReadConfig<ModConfig>();

		// Event subscriptions.
		helper.Events.GameLoop.TimeChanged += TimeChanged;
		helper.Events.GameLoop.GameLaunched += GameLaunched;
	}

	private void GameLaunched(object? sender, GameLaunchedEventArgs e)
	{
		SetupConfig();
	}

	private void TimeChanged(object? sender, TimeChangedEventArgs e)
	{
		Utilities.UpdateEnergy();
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