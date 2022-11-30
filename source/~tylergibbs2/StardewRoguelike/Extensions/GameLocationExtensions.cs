/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewRoguelike.VirtualProperties;
using StardewValley;

namespace StardewRoguelike.Extensions
{
    public static class GameLocationExtensions
    {
        public static void DrawSpeechBubble(this GameLocation location, Vector2 drawPosition, string text, int duration)
        {
            var speechBubbles = location.get_SpeechBubbles();
            speechBubbles[drawPosition] = new(drawPosition, text, duration);
        }

        public static void debuffPlayers(this GameLocation location, Rectangle area, int debuff)
        {
            var debuffEvent = location.get_DebuffPlayerEvent();
            debuffEvent.Fire(area, debuff);
        }

        public static void performDebuffPlayers(this GameLocation location, Rectangle area, int debuff)
        {
            if (Game1.player.currentLocation == location && Game1.player.GetBoundingBox().Intersects(area))
                Game1.buffsDisplay.addOtherBuff(new Buff(debuff));
        }
    }
}
