/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace CropHarvestBubbles
{
    public partial class ModEntry
    {
        [HarmonyPatch(typeof(Crop), nameof(Crop.draw))]
        public class Crop_draw_Patch
        {
            public static void Postfix(Crop __instance, SpriteBatch b, Vector2 tileLocation, Color toTint, float rotation)
            {
                if (!Config.ModEnabled || (Config.RequireKeyPress && !Config.PressKeys.IsDown()) || __instance.forageCrop.Value || __instance.dead.Value || __instance.currentPhase.Value < __instance.phaseDays.Count - 1 || (__instance.fullyGrown.Value && __instance.dayOfCurrentPhase.Value > 0) || !Game1.objectInformation.TryGetValue(__instance.indexOfHarvest.Value, out var value) || (Config.IgnoreFlowers && value.Contains("/Basic -80/")))
                    return;

                float base_sort = (float)((tileLocation.Y + 1) * 64) / 10000f + tileLocation.X / 50000f;
                float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
                float movePercent = (100 - Config.SizePercent) / 100f;
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64 - 8 + movePercent * 40, tileLocation.Y * 64 - 96 - 16 + yOffset + movePercent * 96)), new Rectangle?(new Rectangle(141, 465, 20, 24)), Color.White * (Config.OpacityPercent / 100f), 0f, Vector2.Zero, 4f * (Config.SizePercent / 100f), SpriteEffects.None, base_sort + 1E-06f);

                b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64 + 32, tileLocation.Y * 64 - 64 - 8 + yOffset + movePercent * 56)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, __instance.indexOfHarvest.Value, 16, 16)), Color.White * (Config.OpacityPercent / 100f), 0f, new Vector2(8f, 8f), 4f * (Config.SizePercent / 100f), SpriteEffects.None, base_sort + 1E-05f);
                if (__instance.programColored.Value)
                {
                    b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(tileLocation.X * 64 + 32), (float)(tileLocation.Y * 64 - 64 - 8) + yOffset)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, __instance.indexOfHarvest.Value + 1, 16, 16)), __instance.tintColor.Value * (Config.OpacityPercent / 100f), 0f, new Vector2(8f, 8f), 4f * (Config.SizePercent / 100f), SpriteEffects.None, base_sort + 1.1E-05f);
                }
            }
        }
    }
}