/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spacechase.Shared.Harmony;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace MoreGiantCrops.Patches
{
    /// <summary>Applies Harmony patches to <see cref="GiantCrop"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class GiantCropPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<GiantCrop>(nameof(GiantCrop.draw)),
                prefix: this.GetHarmonyMethod(nameof(Before_Draw))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method which transpiles <see cref="GiantCrop.draw"/>.</summary>
        private static bool Before_Draw(GiantCrop __instance, SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            try
            {
                if (Mod.Sprites.TryGetValue(__instance.parentSheetIndex.Value, out Texture2D tex))
                {
                    double shakeTimer = Mod.Instance.Helper.Reflection.GetField<float>(__instance, "shakeTimer").GetValue();
                    spriteBatch.Draw(tex, Game1.GlobalToLocal(Game1.viewport, tileLocation * 64f - new Vector2(shakeTimer > 0.0 ? (float)(Math.Sin(2.0 * Math.PI / shakeTimer) * 2.0) : 0.0f, 64f)), null, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((tileLocation.Y + 2.0) * 64.0 / 10000.0));
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed in {nameof(GiantCropPatcher)}.{nameof(Before_Draw)}:\n{ex}");
                return true;
            }
        }
    }
}
