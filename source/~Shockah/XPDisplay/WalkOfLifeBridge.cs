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
using StardewValley;

namespace Shockah.XPDisplay
{
	internal static class WalkOfLifeBridge
	{
		private static IImmersiveProfessionsAPI? Api { get; set; }

		private static void SetupIfNeeded()
		{
			if (Api is not null)
				return;
			if (!XPDisplay.Instance.Helper.ModRegistry.IsLoaded("DaLion.ImmersiveProfessions"))
				return;
			Api = XPDisplay.Instance.Helper.ModRegistry.GetApi<IImmersiveProfessionsAPI>("DaLion.ImmersiveProfessions");
		}

		public static bool IsPrestigeEnabled()
		{
			SetupIfNeeded();
			if (Api is null)
				return false;
			return Api.GetConfigs().EnablePrestige;
		}

		public static (Texture2D, Rectangle)? GetExtendedSmallBar()
		{
			SetupIfNeeded();
			if (Api is null)
				return null;
			return (Game1.content.Load<Texture2D>("DaLion.ImmersiveProfessions/SkillBars"), new(0, 0, 7, 9));
		}

		public static (Texture2D, Rectangle)? GetExtendedBigBar()
		{
			SetupIfNeeded();
			if (Api is null)
				return null;
			return (Game1.content.Load<Texture2D>("DaLion.ImmersiveProfessions/SkillBars"), new(16, 0, 13, 9));
		}
	}
}