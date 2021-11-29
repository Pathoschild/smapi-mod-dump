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
using JetBrains.Annotations;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class ObjectGetMinutesForCrystalariumPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ObjectGetMinutesForCrystalariumPatch()
		{
			Original = RequireMethod<SObject>("getMinutesForCrystalarium");
			Postfix = new(GetType(), nameof(ObjectGetMinutesForCrystalariumPostfix));
		}

		#region harmony patches

		/// <summary>Patch to speed up crystalarium processing time for each Gemologist.</summary>
		[HarmonyPostfix]
		private static void ObjectGetMinutesForCrystalariumPostfix(SObject __instance, ref int __result)
		{
			var owner = Game1.getFarmerMaybeOffline(__instance.owner.Value) ?? Game1.MasterPlayer;
			if (owner.HasProfession("Gemologist")) __result = (int) (__result * 0.75);
		}

		#endregion harmony patches
	}
}