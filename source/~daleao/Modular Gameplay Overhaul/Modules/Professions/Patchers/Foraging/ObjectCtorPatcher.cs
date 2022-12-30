/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Foraging;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectCtorPatcher"/> class.</summary>
    internal ObjectCtorPatcher()
    {
        this.Target = this.RequireConstructor<SObject>(
            typeof(Vector2), typeof(int), typeof(string), typeof(bool), typeof(bool), typeof(bool), typeof(bool));
    }

    #region harmony patches

    /// <summary>Patch for Ecologist wild berry recovery.</summary>
    [HarmonyPostfix]
    private static void ObjectCtorPostfix(SObject __instance)
    {
        var owner = Game1.getFarmer(__instance.owner.Value);
        if (__instance.IsWildBerry() && owner.HasProfession(Profession.Ecologist))
        {
            __instance.Edibility =
                (int)(__instance.Edibility * (owner.HasProfession(Profession.Ecologist, true) ? 2f : 1.5f));
        }
    }

    #endregion harmony patches
}
