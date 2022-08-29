/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDrawDuringUsePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponDrawDuringUsePatch()
    {
        Target = RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.drawDuringUse),
            new[]
            {
                typeof(int), typeof(int), typeof(SpriteBatch), typeof(Vector2), typeof(Farmer), typeof(Rectangle),
                typeof(int), typeof(bool)
            });
    }

    #region harmony patches

    /// <summary>Draw weapon during stabby sword lunge.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponDrawDuringUsePrefix(MeleeWeapon __instance, Vector2 ___center,
        int frameOfFarmerAnimation, int facingDirection, SpriteBatch spriteBatch, Vector2 playerPosition, Farmer f,
        Rectangle sourceRect, int type, bool isOnSpecial)
    {
        if (type != MeleeWeapon.stabbingSword || !isOnSpecial) return true; // run original logic

        frameOfFarmerAnimation %= 2;
        switch (facingDirection)
        {
            case Game1.up:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        spriteBatch.Draw(Tool.weaponsTexture, new(playerPosition.X + 64f - 4f, playerPosition.Y - 40f),
                            sourceRect, Color.White, -(float)Math.PI / 4f, ___center, 4f, SpriteEffects.None,
                            Math.Max(0f, (f.getStandingY() - 32) / 10000f));
                        break;
                    case 1:
                        spriteBatch.Draw(Tool.weaponsTexture, new(playerPosition.X + 64f - 16f, playerPosition.Y - 48f),
                            sourceRect, Color.White, -(float)Math.PI / 4f, ___center, 4f, SpriteEffects.None,
                            Math.Max(0f, (f.getStandingY() - 32) / 10000f));
                        break;
                }

                break;
            case Game1.right:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        spriteBatch.Draw(Tool.weaponsTexture, new(playerPosition.X + 64f - 16f, playerPosition.Y - 16f),
                            sourceRect, Color.White, (float)Math.PI / 4f, ___center, 4f, SpriteEffects.None,
                            Math.Max(0f, (f.getStandingY() + 64) / 10000f));
                        break;
                    case 1:
                        spriteBatch.Draw(Tool.weaponsTexture, new(playerPosition.X + 64f - 8f, playerPosition.Y - 24f),
                            sourceRect, Color.White, (float)Math.PI / 4f, ___center, 4f, SpriteEffects.None,
                            Math.Max(0f, (f.getStandingY() + 64) / 10000f));
                        break;
                }

                break;
            case Game1.down:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        spriteBatch.Draw(Tool.weaponsTexture, new(playerPosition.X + 32f, playerPosition.Y - 12f),
                            sourceRect, Color.White, (float)Math.PI * 3f / 4f, ___center, 4f, SpriteEffects.None,
                            Math.Max(0f, (f.getStandingY() + 32) / 10000f));
                        break;
                    case 1:
                        spriteBatch.Draw(Tool.weaponsTexture, new(playerPosition.X + 21f, playerPosition.Y), sourceRect,
                            Color.White, (float)Math.PI * 3f / 4f, ___center, 4f, SpriteEffects.None,
                            Math.Max(0f, (f.getStandingY() + 32) / 10000f));
                        break;
                }

                break;
            case Game1.left:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        spriteBatch.Draw(Tool.weaponsTexture, new(playerPosition.X + 16f, playerPosition.Y - 16f),
                            sourceRect, Color.White, (float)Math.PI * -3f / 4f, ___center, 4f, SpriteEffects.None,
                            Math.Max(0f, (f.getStandingY() + 64) / 10000f));
                        break;
                    case 1:
                        spriteBatch.Draw(Tool.weaponsTexture, new(playerPosition.X + 8f, playerPosition.Y - 24f),
                            sourceRect, Color.White, (float)Math.PI * -3f / 4f, ___center, 4f, SpriteEffects.None,
                            Math.Max(0f, (f.getStandingY() + 64) / 10000f));
                        break;
                }

                break;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}