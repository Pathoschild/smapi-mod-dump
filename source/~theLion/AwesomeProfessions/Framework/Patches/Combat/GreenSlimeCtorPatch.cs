/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

using SuperMode;

#endregion using directives

[UsedImplicitly]
internal class GreenSlimeCtorPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GreenSlimeCtorPatch()
    {
        Original = RequireConstructor<GreenSlime>(typeof(Vector2), typeof(int));
    }

    #region harmony patches

    /// <summary>Patch to increment Slime movement speed based on Eubstance.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeCtorPostfix(GreenSlime __instance)
    {
        var who = __instance.Player;
        if (!who.IsLocalPlayer ||
            ModEntry.PlayerState.Value.SuperMode is not PiperEubstance {ChargeValue: > 0} eubstance) return;

        __instance.addedSpeed += (int) (eubstance.PercentCharge / 0.33f);
    }

    #endregion harmony patches
}