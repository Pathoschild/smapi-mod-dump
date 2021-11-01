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
using StardewValley.Menus;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class SkillsPagePerformHoverActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal SkillsPagePerformHoverActionPatch()
		{
			Original = typeof(SkillsPage).MethodNamed(nameof(SkillsPage.performHoverAction));
			Postfix = new(GetType(), nameof(SkillsPagePerformHoverActionPostfix));
		}

		#region harmony patches

		/// <summary>Patch to truncate profession descriptions in hover menu.</summary>
		[HarmonyPostfix]
		private static void SkillsPagePerformHoverActionPostfix(ref string ___hoverText)
		{
			___hoverText = ___hoverText.Truncate(90);
		}

		#endregion harmony patches
	}
}