/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using Harmony;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions
{
	internal class MeleeWeaponDoAnimateSpecialMovePatch : BasePatch
	{
		private static ILHelper _Helper { get; set; }

		/// <summary>Construct an instance.</summary>
		internal MeleeWeaponDoAnimateSpecialMovePatch()
		{
			_Helper = new ILHelper(Monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(MeleeWeapon), name: "doAnimateSpecialMove"),
				transpiler: new HarmonyMethod(GetType(), nameof(MeleeWeaponDoAnimateSpecialMoveTranspiler))
			);
		}

		#region harmony patches
		/// <summary>Patch remove Acrobat cooldown reduction.</summary>
		protected static IEnumerable<CodeInstruction> MeleeWeaponDoAnimateSpecialMoveTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			_Helper.Attach(instructions).Log($"Patching method {typeof(MeleeWeapon)}::doAnimateSpecialMove.");

			/// Skipped: if (lastUser.professions.Contains(<acrobat_id>) cooldown /= 2

			int i = 0;
			repeat:
			try
			{
				_Helper											// find index of acrobat check
					.FindProfessionCheck(Farmer.acrobat, fromCurrentIndex: i != 0)
					.Retreat(2)
					.GetLabels(out var labels)
					.StripLabels()
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse)	// the false case branch
					)
					.GetOperand(out object isNotAcrobat)		// copy destination
					.Return()
					.Insert(									// insert unconditional branch to skip this check
						new CodeInstruction(OpCodes.Br_S, (Label)isNotAcrobat)
					)
					.Retreat()
					.AddLabels(labels)							// restore labels to inserted branch
					.Advance(3);
			}
			catch (Exception ex)
			{
				_Helper.Error($"Failed while removing vanilla Acrobat cooldown reduction.\nHelper returned {ex}").Restore();
			}

			// repeat injection
			if (++i < 3)
			{
				_Helper.Backup();
				goto repeat;
			}

			return _Helper.Flush();
		}
		#endregion harmony patches
	}
}
