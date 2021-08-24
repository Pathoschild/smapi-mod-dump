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
using StardewModdingAPI;
using StardewValley;
using System;
using System.Linq;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class AnimalHouseAddNewHatchedAnimalPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal AnimalHouseAddNewHatchedAnimalPatch()
		{
			Original = typeof(AnimalHouse).MethodNamed(nameof(AnimalHouse.addNewHatchedAnimal));
			Postfix = new HarmonyMethod(GetType(), nameof(AnimalHouseAddNewHatchedAnimalPostfix));
		}

		#region harmony patches

		/// <summary>Patch for Breeder newborn animals to have random starting friendship.</summary>
		[HarmonyPostfix]
		private static void AnimalHouseAddNewHatchedAnimalPostfix(AnimalHouse __instance)
		{
			try
			{
				var who = Game1.getFarmer(__instance.getBuilding().owner.Value);
				if (!who.HasProfession("Rancher")) return;

				var a = __instance.Animals?.Values.Last();
				if (a == null || a.age.Value != 0 || a.friendshipTowardFarmer.Value != 0) return;
				a.friendshipTowardFarmer.Value = new Random(__instance.GetHashCode() + a.GetHashCode()).Next(0, 200);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}