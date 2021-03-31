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
using System;
using TheLion.Common.Extensions;

namespace TheLion.AwesomeProfessions
{
	internal class AnimalHouseAddNewHatchedAnimalPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal AnimalHouseAddNewHatchedAnimalPatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(AnimalHouse), nameof(AnimalHouse.addNewHatchedAnimal)),
				postfix: new HarmonyMethod(GetType(), nameof(AnimalHouseAddNewHatchedAnimalPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch for Breeder newborn animals to have random starting friendship.</summary>
		protected static void AnimalHouseAddNewHatchedAnimalPostfix(ref AnimalHouse __instance)
		{
			Farmer who = Game1.getFarmer(__instance.getBuilding().owner.Value);
			if (!Utility.SpecificPlayerHasProfession("breeder", who)) return;

			FarmAnimal a = __instance.Animals[__instance.animalsThatLiveHere.Last()];
			if (a.age.Value == 0 && a.friendshipTowardFarmer.Value == 0)
			{
				Random r = new Random(__instance.GetHashCode() + a.GetHashCode());
				a.friendshipTowardFarmer.Value = r.Next(0, 200);
			}
		}
		#endregion harmony patches
	}
}
