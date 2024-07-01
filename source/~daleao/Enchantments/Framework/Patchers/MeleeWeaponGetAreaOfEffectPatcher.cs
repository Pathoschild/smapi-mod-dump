/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

// ReSharper disable PossibleLossOfFraction
namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using System.Reflection;
using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponGetAreaOfEffectPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponGetAreaOfEffectPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MeleeWeaponGetAreaOfEffectPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.getAreaOfEffect));
    }

    #region harmony patches

    /// <summary>Fix stabby lunge hitbox.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponGetAreaOfEffectPrefix(
        MeleeWeapon __instance,
        ref Rectangle __result,
        int x,
        int y,
        int facingDirection,
        ref Vector2 tileLocation1,
        ref Vector2 tileLocation2,
        Rectangle wielderBoundingBox)
    {
        if (__instance.type.Value != MeleeWeapon.defenseSword || !__instance.isOnSpecial ||
            !__instance.hasEnchantmentOfType<StabbingEnchantment>())
        {
            return true; // run original logic
        }

        try
        {
            __result = GetHitboxDuringLunge(
                x,
                y,
                facingDirection,
                ref tileLocation1,
                ref tileLocation2,
                wielderBoundingBox);

            __result.Inflate(__instance.addedAreaOfEffect.Value, __instance.addedAreaOfEffect.Value);
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches

    #region subroutines

    private static Rectangle GetHitboxDuringLunge(
        int x,
        int y,
        int facingDirection,
        ref Vector2 tileLocation1,
        ref Vector2 tileLocation2,
        Rectangle wielderBoundingBox)
    {
        const int width = 74;
        const int height = 64;
        const int upHeightOffset = 42;
        const int horizontalYOffset = -32;
        var hitbox = Rectangle.Empty;
        switch (facingDirection)
        {
            case Game1.up:
                hitbox = new Rectangle(
                    x - (width / 2),
                    wielderBoundingBox.Y - height - upHeightOffset,
                    width / 2,
                    height + upHeightOffset);
                tileLocation1 = new Vector2(
                    (Game1.random.NextDouble() < 0.5 ? hitbox.Left : hitbox.Right) / 64,
                    hitbox.Top / 64);
                tileLocation2 = new Vector2(hitbox.Center.X / 64, hitbox.Top / 64);
                hitbox.Offset(20, -16);
                hitbox.Height += 16;
                hitbox.Width += 20;
                break;
            case Game1.right:
                hitbox = new Rectangle(
                    wielderBoundingBox.Right,
                    y - (height / 2) + horizontalYOffset,
                    height,
                    width);
                tileLocation1 = new Vector2(
                    hitbox.Center.X / 64,
                    (Game1.random.NextDouble() < 0.5 ? hitbox.Top : hitbox.Bottom) / 64);
                tileLocation2 = new Vector2(hitbox.Center.X / 64, hitbox.Center.Y / 64);
                hitbox.Offset(-4, 0);
                hitbox.Width += 16;
                break;
            case Game1.down:
                hitbox = new Rectangle(
                    x - (width / 2),
                    wielderBoundingBox.Bottom,
                    width,
                    height);
                tileLocation1 = new Vector2(
                    (Game1.random.NextDouble() < 0.5 ? hitbox.Left : hitbox.Right) / 64,
                    hitbox.Center.Y / 64);
                tileLocation2 = new Vector2(hitbox.Center.X / 64, hitbox.Center.Y / 64);
                hitbox.Offset(12, -8);
                hitbox.Width -= 21;
                break;
            case Game1.left:
                hitbox = new Rectangle(
                    wielderBoundingBox.Left - height,
                    y - (height / 2) + horizontalYOffset,
                    height,
                    width);
                tileLocation1 = new Vector2(
                    hitbox.Left / 64,
                    (Game1.random.NextDouble() < 0.5 ? hitbox.Top : hitbox.Bottom) / 64);
                tileLocation2 = new Vector2(hitbox.Left / 64, hitbox.Center.Y / 64);
                hitbox.Offset(-12, 0);
                hitbox.Width += 16;
                break;
        }

        return hitbox;
    }

    #endregion subroutines
}
