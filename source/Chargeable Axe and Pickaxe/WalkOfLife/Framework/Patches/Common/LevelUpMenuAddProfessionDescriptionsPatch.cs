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
using StardewModdingAPI;
using System.Collections.Generic;

namespace TheLion.AwesomeProfessions
{
	internal class LevelUpMenuAddProfessionDescriptionsPatch : BasePatch
	{
		private static ITranslationHelper _I18n { get; set; }

		/// <summary>Construct an instance.</summary>
		/// <param name="i18n">Provides localized text.</param>
		internal LevelUpMenuAddProfessionDescriptionsPatch(ITranslationHelper i18n)
		{
			_I18n = i18n;
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(LevelUpMenu), name: "addProfessionDescriptions"),
				prefix: new HarmonyMethod(GetType(), nameof(LevelUpMenuAddProfessionDescriptionsPrefix))
			);
		}

		#region harmony patches
		/// <summary>Patch to apply modded profession descriptions.</summary>
		protected static bool LevelUpMenuAddProfessionDescriptionsPrefix(List<string> descriptions, string professionName)
		{
			if (!Utility.ProfessionMap.Contains(professionName)) return true; // run original logic

			descriptions.Add(_I18n.Get(professionName + ".name"));
			descriptions.AddRange(_I18n.Get(professionName + ".description").ToString().Split('\n'));
			return false; // don't run original logic
		}
		#endregion harmony patches
	}
}
