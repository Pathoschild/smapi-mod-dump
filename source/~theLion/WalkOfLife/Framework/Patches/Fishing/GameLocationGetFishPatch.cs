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
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;
using SUtility = StardewValley.Utility;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class GameLocationGetFishPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GameLocationGetFishPatch()
		{
			Original = typeof(GameLocation).MethodNamed(nameof(GameLocation.getFish));
			Transpiler = new HarmonyMethod(GetType(), nameof(GameLocationGetFishTranspiler));
		}

		#region harmony patches

		/// <summary>Patch for Fisher to reroll reeled fish if first roll resulted in trash.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> GameLocationGetFishTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// Injected: if (ShouldRerollFish(who, whichFish, hasRerolled)) goto <choose_fish>
			///	Before: caught = new Object(whichFish, 1);

			var startOfFishRoll = iLGenerator.DefineLabel();
			var shouldntReroll = iLGenerator.DefineLabel();
			var hasRerolled = iLGenerator.DeclareLocal(typeof(bool));
			var shuffleMethod = typeof(SUtility).GetMethods().Where(mi => mi.Name == "Shuffle").ElementAtOrDefault(1);
			if (shuffleMethod == null)
			{
				Helper.Error($"Failed acquire {typeof(SUtility)}::Shuffle method.");
				return null;
			}

			try
			{
				Helper
					.Insert( // set hasRerolled to false
						new CodeInstruction(OpCodes.Ldc_I4_0),
						new CodeInstruction(OpCodes.Stloc_S, hasRerolled)
					)
					.FindLast( // find index of caught = new Object(whichFish, 1)
						new CodeInstruction(OpCodes.Newobj,
							typeof(SObject).Constructor(new[] { typeof(int), typeof(int), typeof(bool), typeof(int), typeof(int) }))
					)
					.RetreatUntil(
						new CodeInstruction(OpCodes.Ldloc_1)
					)
					.AddLabels(shouldntReroll) // branch here if shouldn't reroll
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = Farmer who
						new CodeInstruction(OpCodes.Ldloc_1), // local 1 = whichFish
						new CodeInstruction(OpCodes.Ldloc_S, hasRerolled),
						new CodeInstruction(OpCodes.Call, typeof(GameLocationGetFishPatch).MethodNamed(nameof(ShouldRerollFish))),
						new CodeInstruction(OpCodes.Brfalse_S, shouldntReroll),
						new CodeInstruction(OpCodes.Ldc_I4_1),
						new CodeInstruction(OpCodes.Stloc_S, hasRerolled), // set hasRerolled to true
						new CodeInstruction(OpCodes.Br, startOfFishRoll)
					)
					.RetreatUntil( // start of choose fish
						new CodeInstruction(OpCodes.Call, shuffleMethod.MakeGenericMethod(typeof(string)))
					)
					.Retreat(2)
					.AddLabels(startOfFishRoll); // branch here to reroll
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding modded Fisher fish reroll.\nHelper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches

		/// <summary>If the first fish roll returned trash, determines whether the farmer is eligible for a reroll.</summary>
		/// <param name="who">The farmer.</param>
		/// <param name="currentFish">The result of the first fish roll.</param>
		/// <param name="hasRerolled">Whether the game has already rerolled once.</param>
		private static bool ShouldRerollFish(Farmer who, int currentFish, bool hasRerolled)
		{
			return !hasRerolled && (166 < currentFish && currentFish < 173 || currentFish == 152 || currentFish == 153 || currentFish == 157)
				&& who.CurrentTool is FishingRod rod
				&& Util.Objects.BaitById.TryGetValue(rod.getBaitAttachmentIndex(), out var baitName)
				&& baitName.AnyOf("Bait", "Wild Bait", "Magic Bait")
				&& who.HasProfession("Fisher");
		}
	}
}