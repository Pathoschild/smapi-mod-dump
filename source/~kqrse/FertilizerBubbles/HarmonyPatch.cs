/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace FertilizerBubbles; 

internal partial class Mod {
    public class HoeDirt_draw_Patch {
        private static bool IsItemFertilizer(Item item) {
            if (item is null) return false;
            if (item.QualifiedItemId == "(O)805") return false; // Tree Fertilizer
            return item.HasContextTag("fertilizer_item") || item.HasContextTag("quality_fertilizer_item");
        }
        
        public static void Postfix(HoeDirt __instance, SpriteBatch spriteBatch) {
            if (!Config.Enabled) return;
            
            if (__instance.HasFertilizer()) return;
            if (Config.HideWhenNoCrop && __instance.crop is null) return;
            
            var currentItem = Game1.player.CurrentItem;
            
            if (Config.DisplayWhenHeld && !IsItemFertilizer(currentItem)) return;
            if (Config.HideWhenUnusable && currentItem is not null && 
                !__instance.CanApplyFertilizer(currentItem.QualifiedItemId)) return;
            if (!Config.DisplayWhenHeld && !ToggleEmoteEnabled) return;
            
            Vector2 tilePosition = __instance.Tile;
            Vector2 emotePosition = Game1.GlobalToLocal(tilePosition * 64);
            float movePercent = (100 - Config.SizePercent) / 100f;
            emotePosition.Y -= 48 - movePercent * 32;
            emotePosition += new Vector2(movePercent * 32 + Config.OffsetX, movePercent * 32 + Config.OffsetY);
                
            spriteBatch.Draw(Game1.emoteSpriteSheet,
                emotePosition, 
                new Rectangle(CurrentEmoteFrame * 16 % Game1.emoteSpriteSheet.Width, 
                    CurrentEmoteFrame * 16 / Game1.emoteSpriteSheet.Width * 16, 
                    16, 
                    16),
                Color.White * (Config.OpacityPercent / 100f), 
                0f,
                Vector2.Zero, 
                4f * Config.SizePercent / 100f, 
                SpriteEffects.None, 
                (tilePosition.Y * 64 + 37) / 10000f);
        }
    }

}