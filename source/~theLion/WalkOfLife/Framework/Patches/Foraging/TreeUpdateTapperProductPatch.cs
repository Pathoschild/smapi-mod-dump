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
using StardewValley.TerrainFeatures;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class TreeUpdateTapperProductPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal TreeUpdateTapperProductPatch()
		{
			Original = RequireMethod<Tree>(nameof(Tree.UpdateTapperProduct));
			Postfix = new(GetType(), nameof(TreeUpdateTapperProductPostfix));
		}

		#region harmony patches

		/// <summary>Patch to decrease syrup production time for Tapper.</summary>
		[HarmonyPostfix]
		private static void TreeUpdateTapperProductPostfix(SObject tapper_instance)
		{
			if (tapper_instance is null) return;

			var owner = Game1.getFarmerMaybeOffline(tapper_instance.owner.Value) ?? Game1.MasterPlayer;
			if (!owner.HasProfession("Tapper")) return;

			if (tapper_instance.MinutesUntilReady > 0)
				tapper_instance.MinutesUntilReady = (int) (tapper_instance.MinutesUntilReady * 0.75);
		}

		#endregion harmony patches
	}
}