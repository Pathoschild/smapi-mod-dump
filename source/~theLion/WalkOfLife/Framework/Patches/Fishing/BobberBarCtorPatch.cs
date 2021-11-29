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
using StardewValley.Menus;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class BobberBarCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal BobberBarCtorPatch()
		{
			Original = RequireConstructor<BobberBar>(typeof(int), typeof(float), typeof(bool), typeof(int));
			Postfix = new(GetType(), nameof(BobberBarCtorPostfix));
		}

		#region harmony patches

		/// <summary>Patch for Aquarist bonus bobber height.</summary>
		[HarmonyPostfix]
		private static void BobberBarCtorPostfix(ref int ___bobberBarHeight, ref float ___bobberBarPos)
		{
			if (!Game1.player.HasProfession("Aquarist")) return;

			var bonusBobberHeight = Utility.Professions.GetAquaristBonusBobberBarHeight();
			___bobberBarHeight += bonusBobberHeight;
			___bobberBarPos -= bonusBobberHeight;
		}

		#endregion harmony patches
	}
}