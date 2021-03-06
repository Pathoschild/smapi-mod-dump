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

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class LevelUpMenuAddProfessionDescriptionsPatch : BasePatch
	{
		private static ITranslationHelper _i18n;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		/// <param name="i18n">Provides localized text.</param>
		internal LevelUpMenuAddProfessionDescriptionsPatch(ModConfig config, IMonitor monitor, ITranslationHelper i18n)
		: base(config, monitor)
		{
			_i18n = i18n;
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

		/// <summary>Patch to apply modded profession descriptions.</summary>
		protected static bool LevelUpMenuAddProfessionDescriptionsPrefix(List<string> descriptions, string professionName)
		{
			if (!Utils.ProfessionsMap.Contains(professionName))
				return true; // run original logic

			descriptions.Add(_i18n.Get(professionName + ".name"));
			descriptions.AddRange(_i18n.Get(professionName + ".description").ToString().Split('\n'));
			return false; // don't run original logic
		}
	}
}
