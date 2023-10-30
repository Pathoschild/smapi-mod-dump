/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewRoguelike.VirtualProperties;
using System;
using System.Runtime.CompilerServices;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(MineShaft), nameof(MineShaft.drawAboveAlwaysFrontLayer))]
    internal class DrawLevelPatch
    {
        public static bool Prefix(MineShaft __instance, SpriteBatch b)
        {
			DrawAbovePatch.Reverse(__instance, b);
			b.End();
			b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
			foreach (NPC i in __instance.characters)
			{
				if (i is Monster monster)
					monster.drawAboveAllLayers(b);
			}
			b.End();
			b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            if (!Game1.game1.takingMapScreenshot && !__instance.isSideBranch())
			{
				int col = __instance.getMineArea() == 0 ? 4 : ((__instance.getMineArea() == 10) ? 6 : ((__instance.getMineArea() == 40) ? 7 : ((__instance.getMineArea() == 80) ? 2 : 3)));
				string txt = __instance.get_MineShaftLevel().Value.ToString();
				Rectangle tsarea = Game1.game1.GraphicsDevice.Viewport.GetTitleSafeArea();
				SpriteText.drawString(b, txt, tsarea.Left + 16, tsarea.Top + 16, 999999, -1, 999999, 1f, 1f, junimoText: false, 2, "", col);
				int text_width = SpriteText.getWidthOfString(txt);
				if (Roguelike.HardMode)
					b.Draw(Game1.mouseCursors, new Vector2(tsarea.Left + 16 + text_width + 16, tsarea.Top + 16) + new Vector2(4f, 6f) * 4f, new Rectangle(192, 324, 7, 10), Color.White, 0f, new Vector2(3f, 5f), 4f + Game1.dialogueButtonScale / 25f, SpriteEffects.None, 1f);
			}

			return false;
		}
    }

	[HarmonyPatch]
	internal class DrawAbovePatch
    {
		[HarmonyReversePatch]
		[HarmonyPatch(typeof(GameLocation), "drawAboveAlwaysFrontLayer")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void Reverse(object instance, SpriteBatch b)
        {
			// its a stub so it has no initial content
			throw new NotImplementedException("It's a stub");
		}
    }
}
