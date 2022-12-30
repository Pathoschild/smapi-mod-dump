/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

// ReSharper disable PossibleLossOfFraction
namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponGetAreaOfEffectPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponGetAreaOfEffectPatcher"/> class.</summary>
    internal MeleeWeaponGetAreaOfEffectPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.getAreaOfEffect));
    }

    #region harmony patches

    /// <summary>Fix Stabbing Sword hitbox during lunge.</summary>
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
        if (__instance.type.Value != MeleeWeapon.stabbingSword || !__instance.isOnSpecial)
        {
            return true; // run original logic
        }

        try
        {
            const int width = 74;
            const int height = 64;
            const int upHeightOffset = 42;
            const int horizontalYOffset = -32;
            switch (facingDirection)
            {
                case Game1.up:
                    __result = new Rectangle(
                        x - (width / 2),
                        wielderBoundingBox.Y - height - upHeightOffset,
                        width / 2,
                        height + upHeightOffset);
                    tileLocation1 = new Vector2(
                        (Game1.random.NextDouble() < 0.5 ? __result.Left : __result.Right) / 64,
                        __result.Top / 64);
                    tileLocation2 = new Vector2(__result.Center.X / 64, __result.Top / 64);
                    __result.Offset(20, -16);
                    __result.Height += 16;
                    __result.Width += 20;
                    break;
                case Game1.right:
                    __result = new Rectangle(
                        wielderBoundingBox.Right,
                        y - (height / 2) + horizontalYOffset,
                        height,
                        width);
                    tileLocation1 = new Vector2(
                        __result.Center.X / 64,
                        (Game1.random.NextDouble() < 0.5 ? __result.Top : __result.Bottom) / 64);
                    tileLocation2 = new Vector2(__result.Center.X / 64, __result.Center.Y / 64);
                    __result.Offset(-4, 0);
                    __result.Width += 16;
                    break;
                case Game1.down:
                    __result = new Rectangle(
                        x - (width / 2),
                        wielderBoundingBox.Bottom,
                        width,
                        height);
                    tileLocation1 = new Vector2(
                        (Game1.random.NextDouble() < 0.5 ? __result.Left : __result.Right) / 64,
                        __result.Center.Y / 64);
                    tileLocation2 = new Vector2(__result.Center.X / 64, __result.Center.Y / 64);
                    __result.Offset(12, -8);
                    __result.Width -= 21;
                    break;
                case Game1.left:
                    __result = new Rectangle(
                        wielderBoundingBox.Left - height,
                        y - (height / 2) + horizontalYOffset,
                        height,
                        width);
                    tileLocation1 = new Vector2(
                        __result.Left / 64,
                        (Game1.random.NextDouble() < 0.5 ? __result.Top : __result.Bottom) / 64);
                    tileLocation2 = new Vector2(__result.Left / 64, __result.Center.Y / 64);
                    __result.Offset(-12, 0);
                    __result.Width += 16;
                    break;
            }

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
}
