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
using StardewValley;
using StardewValley.Locations;
using StardewRoguelike.VirtualProperties;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using Microsoft.Xna.Framework;
using static StardewRoguelike.RoguelikeUtility;
using System;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(GameLocation), "draw")]
    internal class GameLocationDrawPatch
    {
        public static bool Prefix(GameLocation __instance)
        {
            if (__instance is MineShaft mine)
                ChallengeFloor.DrawBeforeLocation(mine, Game1.spriteBatch);

            return true;
        }

        public static void Postfix(GameLocation __instance, SpriteBatch b)
        {
            if (__instance is MineShaft mine)
            {
                foreach (DwarfGate dwarfGate in mine.get_MineShaftDwarfGates())
                    dwarfGate.Draw(b);

                if (Merchant.IsMerchantFloor(mine))
                    Merchant.Draw(mine, b);

                ChallengeFloor.DrawAfterLocation(mine, Game1.spriteBatch);
            }
        }
    }

    [HarmonyPatch(typeof(GameLocation), "drawAboveAlwaysFrontLayer")]
    internal class GameLocationAlwaysFrontDrawPatch
    {
        public static void Postfix(GameLocation __instance, SpriteBatch b)
        {
            foreach (Vector2 drawPosition in __instance.get_SpeechBubbles().Keys)
            {
                SpeechBubble bubble = __instance.get_SpeechBubbles()[drawPosition];
                if (bubble.Duration == 0)
                {
                    __instance.get_SpeechBubbles().Remove(drawPosition);
                    continue;
                }
                Vector2 local = Game1.GlobalToLocal(drawPosition);
                SpriteText.drawStringWithScrollCenteredAt(b, bubble.Text, (int)local.X, (int)local.Y, "", bubble.Alpha, -1, 1, (float)(drawPosition.Y / 10000f + 0.001f + drawPosition.X / 10000f));

                if (bubble.Duration > 30)
                    bubble.Alpha = Math.Min(1f, bubble.Alpha + 0.1f);
                else
                    bubble.Alpha = Math.Max(0f, bubble.Alpha - 0.04f);

                bubble.Duration--;
            }
        }
    }

    [HarmonyPatch(typeof(MineShaft), "drawAboveAlwaysFrontLayer")]
    internal class MineShaftAlwaysFrontDrawPatch
    {
        public static void Postfix(MineShaft __instance, SpriteBatch b)
        {
            foreach (Vector2 drawPosition in __instance.get_SpeechBubbles().Keys)
            {
                SpeechBubble bubble = __instance.get_SpeechBubbles()[drawPosition];
                if (bubble.Duration == 0)
                {
                    __instance.get_SpeechBubbles().Remove(drawPosition);
                    continue;
                }
                Vector2 local = Game1.GlobalToLocal(drawPosition);
                SpriteText.drawStringWithScrollCenteredAt(b, bubble.Text, (int)local.X, (int)local.Y, "", bubble.Alpha, -1, 1, (float)(drawPosition.Y / 10000f + 0.001f + drawPosition.X / 10000f));

                if (bubble.Duration > 30)
                    bubble.Alpha = Math.Min(1f, bubble.Alpha + 0.1f);
                else
                    bubble.Alpha = Math.Max(0f, bubble.Alpha - 0.04f);

                bubble.Duration--;
            }
        }
    }
}
