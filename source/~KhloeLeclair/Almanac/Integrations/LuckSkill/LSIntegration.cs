/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;
	
using Leclair.Stardew.Common.Integrations;

using LuckSkill;

namespace Leclair.Stardew.Almanac.Integrations.LuckSkill {
	public class LSIntegration : BaseAPIIntegration<ILuckSkillApi, ModEntry> { 

		public LSIntegration(ModEntry mod)
		: base(mod, "spacechase0.LuckSkill", "1.2.3") { }

		public bool HasFortunate(Farmer who) {
			return IsLoaded && who.professions.Contains(API.FortunateProfessionId);
		}

		public bool HasPopularHelper(Farmer who) {
			return IsLoaded && who.professions.Contains(API.PopularHelperProfessionId);
		}

		public bool HasLucky(Farmer who) {
			return IsLoaded && who.professions.Contains(API.LuckyProfessionId);
		}

		public bool HasUnUnlucky(Farmer who) {
			return IsLoaded && who.professions.Contains(API.UnUnluckyProfessionId);
		}

		public bool HasShootingStar(Farmer who) {
			return IsLoaded && who.professions.Contains(API.ShootingStarProfessionId);
		}

		public bool HasSpiritChild(Farmer who) {
			return IsLoaded && who.professions.Contains(API.SpiritChildProfessionId);
		}

	}
}
