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
using StardewValley.TerrainFeatures;
using System;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class TreeUpdateTapperProductPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal TreeUpdateTapperProductPatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Tree), nameof(Tree.UpdateTapperProduct)),
				postfix: new HarmonyMethod(GetType(), nameof(TreeUpdateTapperProductPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch to decrease syrup production time for Tapper.</summary>
		protected static void TreeUpdateTapperProductPostfix(SObject tapper_instance)
		{
			if (tapper_instance.heldObject.Value != null && Utility.AnyPlayerHasProfession("tapper", out int n))
			{
				if (tapper_instance.MinutesUntilReady > 0)
					tapper_instance.MinutesUntilReady = (int)(tapper_instance.MinutesUntilReady * Math.Pow(0.75, n));
			}
		}
		#endregion harmony patches
	}
}
