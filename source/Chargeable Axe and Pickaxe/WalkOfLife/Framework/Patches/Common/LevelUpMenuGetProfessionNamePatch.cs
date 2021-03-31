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
using StardewValley.Menus;

namespace TheLion.AwesomeProfessions
{
	internal class LevelUpMenuGetProfessionNamePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuGetProfessionNamePatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(LevelUpMenu), name: "getProfessionName"),
				prefix: new HarmonyMethod(GetType(), nameof(LevelUpMenuGetProfessionNamePrefix))
			);
		}

		#region harmony patches
		/// <summary>Patch to apply modded profession names.</summary>
		protected static bool LevelUpMenuGetProfessionNamePrefix(ref string __result, int whichProfession)
		{
			if (!Utility.ProfessionMap.Contains(whichProfession)) return true; // run original logic

			__result = Utility.ProfessionMap.Reverse[whichProfession];
			return false; // don't run original logic
		}
		#endregion harmony patches
	}

}
