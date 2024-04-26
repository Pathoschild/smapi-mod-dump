/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hunter-Chambers/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.ItemTypeDefinitions;

namespace DeluxeAutoPetter.helpers
{
    internal class DeluxeAutoPetterDrawPatcher
    {
        private static IMonitor? MONITOR;

        internal static void Initialize(IMonitor monitor)
        {
            MONITOR = monitor;
        }

        internal static void ApplyPatch(Harmony harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.draw), new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
                new HarmonyMethod(typeof(DeluxeAutoPetterDrawPatcher), nameof(Draw_Prefix)));
        }

        internal static bool Draw_Prefix(StardewValley.Object __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            try
            {
                // all of this code is taken from the source code for drawing an auto-petter.
                // the only difference is the addition of drawing a second set of rotating hands.

                if (__instance.isTemporarilyInvisible || !Context.IsWorldReady)
                {
                    return true;
                }

                if (__instance.QualifiedItemId.Equals($"(BC){ObjectDetails.GetDeluxeAutoPetterID()}"))
                {
                    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(__instance.QualifiedItemId);
                    Texture2D texture = dataOrErrorItem.GetTexture();
                    Vector2 vector = __instance.getScale();
                    vector *= 4f;
                    Vector2 vector2 = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
                    Rectangle destinationRectangle = new((int)(vector2.X - vector.X / 2f) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(vector2.Y - vector.Y / 2f) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(64f + vector.X), (int)(128f + vector.Y / 2f));
                    float num = Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f;

                    spriteBatch.Draw(texture, destinationRectangle, dataOrErrorItem.GetSourceRect(1, __instance.ParentSheetIndex), Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, num);
                    spriteBatch.Draw(texture, vector2 + new Vector2(8.5f, 12f) * 4f, dataOrErrorItem.GetSourceRect(2, __instance.ParentSheetIndex), Color.White * alpha, (float)Game1.currentGameTime.TotalGameTime.TotalSeconds * -1.5f, new Vector2(7.5f, 15.5f), 4f, SpriteEffects.None, num + 1E-05f);
                    // this is the second set of rotating hands
                    spriteBatch.Draw(texture, vector2 + new Vector2(8.5f, 12f) * 4f, dataOrErrorItem.GetSourceRect(2, __instance.ParentSheetIndex), Color.White * alpha, (float)Game1.currentGameTime.TotalGameTime.TotalSeconds * -1.5f - 1.5f, new Vector2(7.5f, 15.5f), 4f, SpriteEffects.None, num + 1E-05f);

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MONITOR?.Log($"Failed in {nameof(Draw_Prefix)}:\n{ex.Message}", LogLevel.Error);
                return true;
            }
        }
    }
}
