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

using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;


public record struct MachineInfo(
	HashSet<string> KnownMachines
);


public class MachineRuleHandler : DynamicTypeHandler<MachineInfo> {

	public static string[] VANILLA_MACHINES = new string[] {
		"9", // Lightning Rod
		"10", // Bee House
		"12", // Keg
		"13", // Furnace
		"15", // Preserves Jar
		"16", // Cheese Press
		"17", // Loom
		"19", // Oil Maker
		"20", // Recycling Machine
		"21", // Crystalarium
		"24", // Mayonnaise Machine
		"25", // Seed Maker
		"99", // Feed Hopper
		"101", // Incubator
		"104", // Heater
		"105", // Tapper
		"114", // Charcoal Kiln
		"128", // Mushroom Box
		"130", // Chest
		"154", // Worm Bin
		"156", // Slime Incubator
		"158", // Slime Egg-Press
		"163", // Cask
		"165", // Auto-Grabber
		"208", // Workbench
		"209", // Mini-Jukebox
		"211", // Wood Chipper
		"216", // Mini-Fridge
		"232", // Stone Chest
		"238", // Mini-Obelisk
		"239", // Farm Computer
		"248", // Mini Shipping Bin
		"254", // Ostrich Incubator
		"182", // Geode Crusher
		"246", // Coffee Maker
		"247", // Sewing Machine
		"231", // Solar Pannel,
		"90", // Bone Mill
		"256", // Junimo Chest
		"264", // Heavy Tapper
		"265", // Deconstructor
		"272", // Auto-Petter
		"275", // Hopper
	};

	public readonly ModEntry Mod;

	public MachineRuleHandler(ModEntry mod) {
		Mod = mod;
	}

	public override string DisplayName => I18n.Filter_Machine();

	public override string Description => I18n.Filter_Machine_About();

	public override Texture2D Texture => Game1.bigCraftableSpriteSheet;

	public override Rectangle Source => SObject.getSourceRectForBigCraftable(20);

	public override bool AllowMultiple => false;

	public override bool HasEditor => false;

	public override bool DoesRecipeMatch(IRecipe recipe, Lazy<Item?> item, MachineInfo state) {

		if (item.Value is not SObject sobj || !sobj.bigCraftable.Value)
			return false;

		string key = $"{sobj.ParentSheetIndex}";

		return state.KnownMachines.Contains(key);
	}

	public override IClickableMenu? GetEditor(IClickableMenu parent, IDynamicRuleData type) {
		return null;
	}

	public override IFlowNode[]? GetExtraInfo(MachineInfo state) {
		return null;
	}

	public override MachineInfo ParseStateT(IDynamicRuleData type) {
		HashSet<string> known = Mod.intPFM!.GetMachineIDs() ?? new();

		foreach (string machine in VANILLA_MACHINES)
			known.Add(machine);

		return new MachineInfo(known);
	}
}
