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
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions
{
	internal class LevelUpMenuRevalidateHealthPatch : BasePatch
	{
		private static ILHelper _Helper { get; set; }

		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuRevalidateHealthPatch()
		{
			_Helper = new ILHelper(Monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.RevalidateHealth)),
				transpiler: new HarmonyMethod(GetType(), nameof(LevelUpMenuRevalidateHealthTranspiler)),
				postfix: new HarmonyMethod(GetType(), nameof(LevelUpMenuRevalidateHealthPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch to move bonus health from Defender to Brute.</summary>
		protected static IEnumerable<CodeInstruction> LevelUpMenuRevalidateHealthTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			_Helper.Attach(instructions).Log($"Patching method {typeof(LevelUpMenu)}::{nameof(LevelUpMenu.RevalidateHealth)}.");

			/// From: if (farmer.professions.Contains(<defender_id>))
			/// To: if (farmer.professions.Contains(<brute_id>))

			try
			{
				_Helper
					.FindProfessionCheck(Farmer.defender)
					.Advance()
					.SetOperand(Utility.ProfessionMap.Forward["brute"]);
			}
			catch (Exception ex)
			{
				_Helper.Error($"Failed while moving vanilla Defender health bonus to Brute.\nHelper returned {ex}").Restore();
			}

			return _Helper.Flush();
		}

		/// <summary>Patch revalidate modded immediate profession perks.</summary>
		protected static void LevelUpMenuRevalidateHealthPostfix(Farmer farmer)
		{
			// revalidate tackle health
			int expectedMaxTackleUses = 20;
			if (Utility.SpecificPlayerHasProfession("angler", farmer)) expectedMaxTackleUses *= 2;

			FishingRod.maxTackleUses = expectedMaxTackleUses;

			// revalidate fish pond capacity
			foreach (Building b in Game1.getFarm().buildings)
			{
				if ((b.owner.Equals(farmer.UniqueMultiplayerID) || !Game1.IsMultiplayer) && b is FishPond)
				{
					(b as FishPond).UpdateMaximumOccupancy();
					b.currentOccupants.Value = Math.Min(b.currentOccupants.Value, b.maxOccupants.Value);
				}
			}
		}
		#endregion harmony patches
	}
}
