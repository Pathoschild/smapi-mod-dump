/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Mining;

#region using directives

using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectGetMinutesForCrystalariumPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectGetMinutesForCrystalariumPatch()
    {
        Target = RequireMethod<SObject>("getMinutesForCrystalarium");
    }

    #region harmony patches

    /// <summary>Patch to speed up crystalarium processing time for each Gemologist.</summary>
    [HarmonyPostfix]
    private static void ObjectGetMinutesForCrystalariumPostfix(SObject __instance, ref int __result)
    {
        var owner = Game1.getFarmerMaybeOffline(__instance.owner.Value) ?? Game1.MasterPlayer;
        if (owner.HasProfession(Profession.Gemologist))
            __result = (int)(__result * (owner.HasProfession(Profession.Gemologist, true) ? 0.5 : 0.75));
    }

    #endregion harmony patches
}