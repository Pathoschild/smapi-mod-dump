/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Runtime.CompilerServices;

using AtraBase.Toolkit;

using CritterRings;

using HarmonyLib;

using StardewValley.Tools;

/// <summary>
/// Patches to make sure the player doesn't move in certain times.
/// </summary>
[HarmonyPatch]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Named for Harmony.")]
internal static class JumpPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.leftClick))]
    private static bool PrefixSwordSwing(Farmer who)
    {
        return ModEntry.CurrentJumper?.IsValid(out Farmer? farmer) != true || !ReferenceEquals(who, farmer);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Game1), nameof(Game1.pressUseToolButton))]
    private static bool PrefixUseTool()
    {
        return ModEntry.CurrentJumper?.IsValid(out Farmer? farmer) != true;
    }

    [HarmonyPostfix]
    [MethodImpl(TKConstants.Hot)]
    [HarmonyPatch(typeof(Farmer), nameof(Farmer.getDrawLayer))]
    private static void PostfixGetDrawLayer(Farmer __instance, ref float __result)
    {
        switch (MathF.Sign(__instance.yJumpVelocity))
        {
            // player rising.
            case 1:

                // and moving forward
                if (MathF.Sign(__instance.Position.Y - __instance.lastPosition.Y) == 1)
                {
                    __result -= 0.0035f;
                    return;
                }

                __result += 0.0035f;
                return;

            // player falling
            case -1:

                // and moving backwards
                if (MathF.Sign(__instance.Position.Y - __instance.lastPosition.Y) == -1)
                {
                    __result -= 0.0035f;
                    return;
                }

                __result += 0.0035f;
                return;
                break;
        }
    }
}