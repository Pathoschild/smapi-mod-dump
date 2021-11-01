/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Classes;

namespace TheLion.Stardew.Tools.Framework
{
	/// <summary>Patches the game code to implement modded tool behavior.</summary>
	internal static class HarmonyPatcher
	{
		private static readonly List<int> AxeAffectedTilesRadii = ModEntry.Config.AxeConfig.RadiusAtEachPowerLevel;
		private static readonly List<int> PickaxeAffectedTilesRadii = ModEntry.Config.PickaxeConfig.RadiusAtEachPowerLevel;

		// Enable Axe power level increase
		[HarmonyPatch(typeof(Axe), "beginUsing")]
		internal class BeforeAxeBeginUsing
		{
			protected static bool Prefix(ref Tool __instance, Farmer who)
			{
				if (!Utility.ShouldCharge() || !ModEntry.Config.AxeConfig.EnableAxeCharging || __instance.UpgradeLevel < ModEntry.Config.AxeConfig.RequiredUpgradeForCharging)
					return true; // run original logic

				who.Halt();
				__instance.Update(who.FacingDirection, 0, who);
				switch (who.FacingDirection)
				{
					case 0:
						who.FarmerSprite.setCurrentFrame(176);
						__instance.Update(0, 0, who);
						break;

					case 1:
						who.FarmerSprite.setCurrentFrame(168);
						__instance.Update(1, 0, who);
						break;

					case 2:
						who.FarmerSprite.setCurrentFrame(160);
						__instance.Update(2, 0, who);
						break;

					case 3:
						who.FarmerSprite.setCurrentFrame(184);
						__instance.Update(3, 0, who);
						break;
				}

				return false; // don't run original logic
			}
		}

		// Enable Pickaxe power level increase
		[HarmonyPatch(typeof(Pickaxe), "beginUsing")]
		internal class BeforePickaxeBeginUsing
		{
			protected static bool Prefix(ref Tool __instance, Farmer who)
			{
				if (!Utility.ShouldCharge() || !ModEntry.Config.PickaxeConfig.EnablePickaxeCharging || __instance.UpgradeLevel < ModEntry.Config.PickaxeConfig.RequiredUpgradeForCharging)
					return true; // run original logic

				who.Halt();
				__instance.Update(who.FacingDirection, 0, who);
				switch (who.FacingDirection)
				{
					case 0: // up
						who.FarmerSprite.setCurrentFrame(176);
						__instance.Update(0, 0, who);
						break;

					case 1: // right
						who.FarmerSprite.setCurrentFrame(168);
						__instance.Update(1, 0, who);
						break;

					case 2: // down
						who.FarmerSprite.setCurrentFrame(160);
						__instance.Update(2, 0, who);
						break;

					case 3: // left
						who.FarmerSprite.setCurrentFrame(184);
						__instance.Update(3, 0, who);
						break;
				}

				return false; // don't run original logic
			}
		}

		// Allow first two power levels on Pickaxe
		[HarmonyPatch(typeof(Farmer), "toolPowerIncrease")]
		internal class DuringFarmerToolPowerIncrease
		{
			protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				var l = instructions.ToList();
				for (int i = 0; i < l.Count; ++i)
				{
					if (l[i].opcode == OpCodes.Isinst && l[i].operand?.ToString() == "StardewValley.Tools.Pickaxe")
					{
						// inject branch over toolPower += 2
						l.Insert(i - 2, new CodeInstruction(OpCodes.Br_S, l[i + 1].operand));
						break;
					}
				}

				return l.AsEnumerable();
			}
		}

		// Set affected tiles for Axe and Pickaxe power levels
		[HarmonyPatch(typeof(Tool), "tilesAffected")]
		internal class AfterToolTileseAffected
		{
			protected static void Postfix(ref Tool __instance, ref List<Vector2> __result, Vector2 tileLocation, int power)
			{
				if (__instance.UpgradeLevel < Tool.copper)
					return;

				if (__instance is Axe or Pickaxe)
				{
					__result.Clear();

					int radius = __instance is Axe ? AxeAffectedTilesRadii[Math.Min(power - 2, 4)] : PickaxeAffectedTilesRadii[Math.Min(power - 2, 4)];
					if (radius == 0)
						return;

					var grid = new CircleTileGrid(tileLocation, radius);
					__result.AddRange(grid);
				}
			}
		}

		// Hide affected tiles overlay for Axe or Pickaxe
		[HarmonyPatch(typeof(Tool), "draw")]
		internal class BeforeToolDraw
		{
			protected static bool Prefix(ref Tool __instance)
			{
				return (__instance is not Axe || ModEntry.Config.AxeConfig.ShowAxeAffectedTiles) && (__instance is not Pickaxe || ModEntry.Config.PickaxeConfig.ShowPickaxeAffectedTiles);
			}
		}

		// Enable Scythe power level increase
		[HarmonyPatch(typeof(Tool), "beginUsing")]
		internal class BeforeToolBeginUsing
		{
			protected static bool Prefix(ref Tool __instance, Farmer who)
			{
				if (!Utility.ShouldCharge() || __instance is not MeleeWeapon weapon || !weapon.isScythe() || !__instance.Name.Contains("Golden") || !ModEntry.Config.ScytheConfig.EnableScytheCharging)
					return true; // run original logic

				who.Halt();
				__instance.Update(who.FacingDirection, 0, who);
				switch (who.FacingDirection)
				{
					case 0:
						who.FarmerSprite.setCurrentFrame(248);
						__instance.Update(0, 0, who);
						break;

					case 1:
						who.FarmerSprite.setCurrentFrame(240);
						__instance.Update(1, 0, who);
						break;

					case 2:
						who.FarmerSprite.setCurrentFrame(232);
						__instance.Update(2, 0, who);
						break;

					case 3:
						who.FarmerSprite.setCurrentFrame(256);
						__instance.Update(3, 0, who);
						break;
				}

				ModEntry.Reflection.GetField<bool>(who, name: "canReleaseTool").SetValue(true);

				return false; // don't run original logic
			}
		}
	}
}