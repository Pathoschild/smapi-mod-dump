/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Foraging;

#region using directives

using Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectCtorPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectCtorPatch()
    {
        Target = RequireConstructor<SObject>(typeof(Vector2), typeof(int), typeof(string), typeof(bool),
            typeof(bool), typeof(bool), typeof(bool));
    }

    #region harmony patches

    /// <summary>Patch for Ecologist wild berry recovery.</summary>
    [HarmonyPostfix]
    private static void ObjectCtorPostfix(SObject __instance)
    {
        var owner = Game1.getFarmer(__instance.owner.Value);
        if (__instance.IsWildBerry() && owner.HasProfession(Profession.Ecologist))
            __instance.Edibility =
                (int)(__instance.Edibility * (owner.HasProfession(Profession.Ecologist, true) ? 2f : 1.5f));
    }

    #endregion harmony patches
}