/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using Leclair.Stardew.Common.Integrations;

using SpaceCore;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Integrations.SpaceCore {
	public class SCIntegration : BaseAPIIntegration<IApi, ModEntry> {

		public SCIntegration(ModEntry mod)
		: base(mod, "spacechase0.SpaceCore", "1.8.1") {

		}

		public void AddCustomSkillExperience(Farmer farmer, string skill, int amt) {
			if (!IsLoaded)
				return;

			API.AddExperienceForCustomSkill(farmer, skill, amt);
		}

		public int GetCustomSkillLevel(Farmer farmer, string skill) {
			if (!IsLoaded)
				return 0;

			return API.GetLevelForCustomSkill(farmer, skill);
		}

	}
}
