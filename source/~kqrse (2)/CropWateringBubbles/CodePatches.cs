/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using StardewValley.TerrainFeatures;

namespace CropWateringBubbles
{
    public partial class ModEntry
    {
        public class HoeDirt_draw_Patch
        {
            public static void Postfix(HoeDirt __instance, SpriteBatch spriteBatch)
            {
                if (!Config.ModEnabled ||
                    (!isEmoting && Config.RepeatInterval != 0) || 
                    __instance.crop is null ||
                    __instance.crop.dead.Value ||
                    __instance.state.Value != 0 ||
                    (__instance.crop.currentPhase.Value >= __instance.crop.phaseDays.Count - 1 && (!__instance.crop.fullyGrown.Value || __instance.crop.dayOfCurrentPhase.Value <= 0) && !CanBecomeGiant(__instance)) ||
                    (Config.OnlyWhenWatering && Game1.player.CurrentTool is not WateringCan))
                    return;

                Vector2 tilePosition = __instance.Tile;
                Vector2 emotePosition = Game1.GlobalToLocal(tilePosition * 64);
                float movePercent = (100 - Config.SizePercent) / 100f;
                emotePosition.Y -= 48 - movePercent * 32;
                emotePosition += new Vector2(movePercent * 32, movePercent * 32);
                
                spriteBatch.Draw(Game1.emoteSpriteSheet,
                    emotePosition, 
                    new Rectangle(currentEmoteFrame * 16 % Game1.emoteSpriteSheet.Width, currentEmoteFrame * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16),
                    Color.White * (Config.OpacityPercent / 100f), 
                    0f,
                    Vector2.Zero, 
                    4f * Config.SizePercent / 100f, 
                    SpriteEffects.None, 
                    (tilePosition.Y * 64 + 37) / 10000f);
            }

        }
    }
}