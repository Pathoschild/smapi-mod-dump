/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;

namespace WheresMyHorse;

internal partial class Mod {
    public class Horse_draw_Patch {
        [HarmonyAfter("Goldenrevolver.HorseOverhaul")]
        public static void Postfix(Horse __instance, SpriteBatch b) {
            if (!Config.Enabled) return;
            if (__instance.rider != null) return;
            if (Config.OnlyMyHorse && __instance.getOwner() != Game1.player) return; 
            if (!Config.AlwaysRender && !EmoteEnabled) return;

            float offsetX = Config.OffsetX + (__instance.GetSpriteWidthForPositioning() == 16 ? 0f : 32f);
            float offsetY = Config.OffsetY - 96f;
            Vector2 localPosition = __instance.getLocalPosition(Game1.viewport) + new Vector2(offsetX, offsetY);
            
            float num = __instance.StandingPixel.Y + 1;

            switch (__instance.FacingDirection)
            {
                case 0:
                    localPosition.Y -= 40f;
                    break;
                case 1:
                    localPosition.X += 40f;
                    localPosition.Y -= 30f;
                    break;
                case 2:
                    localPosition.Y += 5f;
                    break;
                case 3:
                    localPosition.X -= 40f;
                    localPosition.Y -= 30f;
                    break;
            }

            b.Draw(Game1.emoteSpriteSheet,
                localPosition,
                new Rectangle(CurrentEmoteFrame * 16 % Game1.emoteSpriteSheet.Width,
                    CurrentEmoteFrame * 16 / Game1.emoteSpriteSheet.Width * 16, 
                    16, 
                    16), 
                Color.White  * (Config.OpacityPercent / 100f), 
                0.0f, 
                Vector2.Zero,
                4f * Config.SizePercent / 100f, 
                SpriteEffects.None, 
                Config.RenderOnTop ? 0.99f : num / 10000f);
        }
    }
}