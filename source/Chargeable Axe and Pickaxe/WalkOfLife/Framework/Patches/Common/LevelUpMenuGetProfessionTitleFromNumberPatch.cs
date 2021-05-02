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
using System;

namespace TheLion.AwesomeProfessions
{
	internal class LevelUpMenuGetProfessionTitleFromNumberPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.getProfessionTitleFromNumber)),
				prefix: new HarmonyMethod(GetType(), nameof(LevelUpMenuGetProfessionTitleFromNumberPrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch to apply modded profession names.</summary>
		private static bool LevelUpMenuGetProfessionTitleFromNumberPrefix(ref string __result, int whichProfession)
		{
			try
			{
				if (!Utility.ProfessionMap.Contains(whichProfession)) return true; // run original logic

				__result = AwesomeProfessions.I18n.Get(Utility.ProfessionMap.Reverse[whichProfession] + ".name");
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(LevelUpMenuGetProfessionTitleFromNumberPrefix)}:\n{ex}");
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}