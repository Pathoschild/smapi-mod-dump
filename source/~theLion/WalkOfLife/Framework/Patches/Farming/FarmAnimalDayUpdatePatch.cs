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
using Netcode;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class FarmAnimalDayUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FarmAnimalDayUpdatePatch()
		{
			Original = typeof(FarmAnimal).MethodNamed(nameof(FarmAnimal.dayUpdate));
			Transpiler = new HarmonyMethod(GetType(), nameof(FarmAnimalDayUpdateTranspiler));
		}

		#region harmony patches

		/// <summary>Patch for Producer to double produce frequency at max animal happiness + remove Shepherd and Coopmaster hidden produce quality boosts.</summary>
		[HarmonyTranspiler]
		protected static IEnumerable<CodeInstruction> FarmAnimalDayUpdateTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// From: FarmeAnimal.daysToLay -= (FarmAnimal.type.Value.Equals("Sheep") && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(Farmer.shepherd)) ? 1 : 0
			/// To: FarmAnimal.daysToLay /= (FarmAnimal.happiness.Value >= 200) && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<producer_id>) ? 2 : 1

			try
			{
				Helper
					.FindFirst( // find index of FarmAnimal.type.Value.Equals("Sheep")
						new CodeInstruction(OpCodes.Ldstr, "Sheep"),
						new CodeInstruction(OpCodes.Callvirt,
							typeof(string).MethodNamed(nameof(string.Equals), new[] { typeof(string) }))
					)
					.Retreat(2)
					.SetOperand(typeof(FarmAnimal).Field(nameof(FarmAnimal.happiness))) // was FarmAnimal.type
					.Advance()
					.SetOperand(typeof(NetFieldBase<byte, NetByte>)
						.PropertyGetter(nameof(NetFieldBase<byte, NetByte>.Value))) // was <string, NetString>
					.Advance()
					.ReplaceWith(
						new CodeInstruction(OpCodes.Ldc_I4_S, 200) // was Ldstr "Sheep"
					)
					.Advance()
					.Remove()
					.SetOpCode(OpCodes.Blt_S) // was Brfalse
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_0)
					)
					.SetOpCode(OpCodes.Ldc_I4_1) // was Ldc_I4_0
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_1)
					)
					.SetOpCode(OpCodes.Ldc_I4_2) // was Ldc_I4_1
					.Advance()
					.SetOpCode(OpCodes.Div) // was Sub
					.Advance()
					.Insert(
						new CodeInstruction(OpCodes.Conv_U1)
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while patching modded Producer produce frequency.\nHelper returned {ex}");
				return null;
			}

			/// Skipped: if ((!isCoopDweller() && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<shepherd_id>)) || (isCoopDweller() && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<coopmaster_id>))) chanceForQuality += 0.33

			try
			{
				Helper
					.FindNext( // find index of first FarmAnimal.isCoopDweller check
						new CodeInstruction(ModEntry.GameFramework.Equals(GameFramework.Xna) ? OpCodes.Call : OpCodes.Callvirt,
							typeof(FarmAnimal).MethodNamed(nameof(FarmAnimal.isCoopDweller)))
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse) // the all cases false branch
					)
					.GetOperand(out var resumeExecution) // copy destination
					.Return()
					.Retreat()
					.Insert( // insert unconditional branch to skip this whole section
						new CodeInstruction(OpCodes.Br_S, (Label)resumeExecution)
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while removing vanilla Coopmaster + Shepherd produce quality bonuses.\nHelper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}