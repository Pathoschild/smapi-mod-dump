/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/personal-indoor-farm
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalIndoorFarm.Lib
{
    internal class Painting
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public const string ItemId = "DLX.PIF_Painting";
        public const string QualifiedItemId = "(F)" + ItemId;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;

            var harmony = new Harmony(Mod.ModManifest.UniqueID);
            
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Furniture), nameof(Furniture.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
                prefix: new HarmonyMethod(typeof(Painting), nameof(Painting.draw_Prefix))
            );
        }

        public static bool draw_Prefix(Furniture __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {

            if (__instance.QualifiedItemId != QualifiedItemId)
                return true;

            try {
                var season = Game1.currentLocation.GetSeason();
                var offset = season switch {
                    Season.Spring => 0,
                    Season.Summer => 1,
                    Season.Fall => 2,
                    Season.Winter => 3,
                    _ => 0
                };
                var day = Game1.currentLocation.Name.StartsWith(PersonalFarm.BaseLocationKey) ? PersonalFarm.getDayOfMonth(Game1.currentLocation) : Game1.dayOfMonth;

                Rectangle drawn_source_rect = __instance.sourceRect.Value;
                drawn_source_rect.X += drawn_source_rect.Width * offset;

                ParsedItemData itemData = ItemRegistry.GetDataOrErrorItem(__instance.QualifiedItemId);
                Texture2D texture = itemData.GetTexture();
                string textureName = itemData.TextureName;

                var drawPosition = Helper.Reflection.GetField<Netcode.NetVector2>(__instance, "drawPosition").GetValue();

                Vector2 actualDrawPosition = Game1.GlobalToLocal(Game1.viewport, drawPosition.Value + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero));
                SpriteEffects spriteEffects = (__instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                Color color = Color.White * alpha;

                var layerdepth = ((int)__instance.furniture_type.Value == 12) ? (2E-09f + __instance.TileLocation.Y / 100000f) : ((float)(__instance.boundingBox.Value.Bottom - (((int)__instance.furniture_type.Value == 6 || (int)__instance.furniture_type.Value == 17 || (int)__instance.furniture_type.Value == 13) ? 48 : 8)) / 10000f);
                spriteBatch.Draw(texture, actualDrawPosition, drawn_source_rect, color, 0f, Vector2.Zero, 4f, spriteEffects, layerdepth);

                var digitOffset = day > 9 ?
                    new Vector2(12, 46) :
                    new Vector2(20, 46);
                Utility.drawTinyDigits(day, spriteBatch, actualDrawPosition + digitOffset, 4f, layerdepth + 0.00001f, Color.White * alpha * 0.75f);

                return false;
            } catch(Exception err) {
                Monitor.LogOnce(err.Message, LogLevel.Error);
            }
            return true;
        }
    }
}
