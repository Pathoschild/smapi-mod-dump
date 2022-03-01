/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.BetterArtisanGoodIcons;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Objects;

using Utility;

#endregion using directives

[UsedImplicitly]
internal class FurnitureDrawPatch : BasePatch
{
	/// <summary>Construct an instance.</summary>
    internal FurnitureDrawPatch()
    {
        Original = ModEntry.ModHelper.ModRegistry.IsLoaded("cat.betterartisangoodicons")
            ? RequireMethod<Furniture>(nameof(Furniture.draw),
                new[] {typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)})
            : null;
        Prefix.before = new[] { "cat.betterartisangoodicons" };
    }

    #region harmony patches

    /// <summary>Patch to draw BAGI-like meads on furniture.</summary>
    /// <remarks>Credit to <c>danvolchek (a.k.a. Cat)</c>.</remarks>
    [HarmonyPrefix]
    [HarmonyBefore("cat.betterartisangoodicons")]
    private static bool FurnitureDrawPrefix(Furniture __instance, NetVector2 ___drawPosition, SpriteBatch spriteBatch, int x, int y,
        float alpha = 1f)
    {
        if (__instance.heldObject.Value is not {ParentSheetIndex: 459, preservedParentSheetIndex.Value: > 0} mead ||
            !Textures.TryGetMeadSourceRect(mead.preservedParentSheetIndex.Value, out var sourceRect)) return true; // run original logic

        // draw the furniture
        if (x == -1)
        {
            spriteBatch.Draw(
                Furniture.furnitureTexture,
                Game1.GlobalToLocal(Game1.viewport, ___drawPosition),
                __instance.sourceRect.Value,
                Color.White * alpha,
                0f,
                Vector2.Zero,
                4f,
                __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                __instance.furniture_type.Value == 12
                    ? 0f
                    : (__instance.boundingBox.Bottom - 8) / 10000f
            );
        }
        else
        {
            spriteBatch.Draw(
                Furniture.furnitureTexture,
                Game1.GlobalToLocal(Game1.viewport,
                    new Vector2(x * 64, y * 64 - (__instance.sourceRect.Height * 4 - __instance.boundingBox.Height))),
                __instance.sourceRect.Value,
                Color.White * alpha,
                0f,
                Vector2.Zero,
                4f,
                __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                __instance.furniture_type.Value == 12
                    ? 0f
                    : (__instance.boundingBox.Bottom - 8) / 10000f
            );
        }

        // draw shadow
        spriteBatch.Draw(
            Game1.shadowTexture,
            Game1.GlobalToLocal(Game1.viewport,
                new Vector2(__instance.boundingBox.Value.Center.X - 32,
                    __instance.boundingBox.Value.Center.Y - (__instance.drawHeldObjectLow.Value ? 32 : 85))) +
            new Vector2(32f, 53.3333321f),
            Game1.shadowTexture.Bounds,
            Color.White * alpha,
            0f,
            new(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
            4f,
            SpriteEffects.None, __instance.boundingBox.Value.Bottom / 10000f
        );
        
        // draw the held item
        spriteBatch.Draw(
            Textures.HoneyMeadTx,
            Game1.GlobalToLocal(Game1.viewport,
                new Vector2(__instance.boundingBox.Value.Center.X - 32,
                    __instance.boundingBox.Value.Center.Y - (__instance.drawHeldObjectLow.Value ? 32 : 85))),
            sourceRect,
            Color.White * alpha,
            0f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            (__instance.boundingBox.Value.Bottom + 1) / 10000f
        );
        
        return false; // run original logic
    }

    #endregion harmony patches
}