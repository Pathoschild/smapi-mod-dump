/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using StardewValley;

namespace Leclair.Stardew.BCBuildings;

public class BlueprintPopulationEventArgs: EventArgs, IBlueprintPopulationEventArgs {

	public Dictionary<string, BluePrint> Blueprints { get; }

	internal BlueprintPopulationEventArgs(Dictionary<string, BluePrint> blueprints) {
		Blueprints = blueprints;
	}
}

public class ModApi : IBCBuildingsApi {

	private readonly ModEntry Mod;

	internal ModApi(ModEntry mod) {
		Mod = mod;
	}

	public void AddBlueprint(string name, string? displayCondition, string? buildCondition) {
		AddBlueprint(new BluePrint(name), displayCondition, buildCondition);
	}

	public void AddBlueprint(BluePrint blueprint, string? displayCondition, string? buildCondition) {
		Mod.ApiBlueprints[blueprint.name] = (blueprint, displayCondition, buildCondition);
	}

	public void RemoveBlueprint(string name) {
		if (Mod.ApiBlueprints.ContainsKey(name))
			Mod.ApiBlueprints.Remove(name);
	}

	public void RemoveBlueprint(BluePrint blueprint) {
		RemoveBlueprint(blueprint.name);
	}

	public void SetTextureSourceRect(string name, Rectangle? rect) {
		if (string.IsNullOrEmpty(name))
			return;

		if (rect.HasValue)
			Mod.BuildingSources[name] = rect;
		else if (Mod.BuildingSources.ContainsKey(name))
			Mod.BuildingSources.Remove(name);
	}

	public event EventHandler<IBlueprintPopulationEventArgs>? OnBlueprintPopulation;

	internal void EmitBlueprintPopulation(Dictionary<string, BluePrint> blueprints) {
		if (OnBlueprintPopulation is null)
			return;

		var evt = new BlueprintPopulationEventArgs(blueprints);
		OnBlueprintPopulation.Invoke(this, evt);
	}

}
