/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Patches.Combat;

#region using directives

using Common.Integrations.WalkOfLife;
using Enchantments;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotGetAutoFireRatePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotGetAutoFireRatePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.GetAutoFireRate));
    }

    #region harmony patches

    /// <summary>Implement <see cref="GatlingEnchantment"/> effect.</summary>
    [HarmonyPostfix]
    private static void SlingshotGetAutoFireRatePostfix(Slingshot __instance, ref float __result)
    {
        var ultimate = ModEntry.ProfessionsApi?.GetRegisteredUltimate();
        if (ultimate is not null && ultimate.Index == IImmersiveProfessions.UltimateIndex.Blossom &&
            ultimate.IsActive || !__instance.hasEnchantmentOfType<GatlingEnchantment>()) return;

        __result *= 1.5f;
    }

    #endregion harmony patches
}