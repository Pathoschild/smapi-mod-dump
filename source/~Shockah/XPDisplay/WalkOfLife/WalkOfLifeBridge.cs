/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Kokoro.Stardew;
using Shockah.XPDisplay.WalkOfLife;
using StardewValley;
using StardewValley.Constants;

namespace Shockah.XPDisplay.WalkOfLife
{
	internal static class WalkOfLifeBridge
	{
		private static IProfessionsApi? Api { get; set; }

		private static void SetupIfNeeded()
		{
			if (Api is not null)
				return;
			if (!XPDisplay.Instance.Helper.ModRegistry.IsLoaded("DaLion.Professions"))
				return;
			Api = XPDisplay.Instance.Helper.ModRegistry.GetApi<IProfessionsApi>("DaLion.Professions");
		}

		public static bool IsPrestigeEnabled(ISkill skill)
		{
			SetupIfNeeded();
			if (Api is null)
				return false;

			//WOL requires mastering a skill to unlock prestige progression
			if (skill is not VanillaSkill vanillaSkill || Game1.player.stats.Get(StatKeys.Mastery(vanillaSkill.SkillIndex)) == 0)
				return false;


			return Api.GetConfig().Masteries.EnablePrestigeLevels;
		}

		public static (Texture2D, Rectangle)? GetExtendedSmallBar()
		{
			SetupIfNeeded();
			if (Api is null)
				return null;
			return (Game1.content.Load<Texture2D>("DaLion.Professions/SkillBars"), new(0, 0, 7, 9));
		}

		public static (Texture2D, Rectangle)? GetExtendedBigBar()
		{
			SetupIfNeeded();
			if (Api is null)
				return null;
			return (Game1.content.Load<Texture2D>("DaLion.Professions/SkillBars"), new(16, 0, 13, 9));
		}
	}
}