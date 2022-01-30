/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Farming;

#region using directives

using System;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class AnimalHouseAddNewHatchedAnimalPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal AnimalHouseAddNewHatchedAnimalPatch()
    {
        Original = RequireMethod<AnimalHouse>(nameof(AnimalHouse.addNewHatchedAnimal));
    }

    #region harmony patches

    /// <summary>Patch for Breeder newborn animals to have random starting friendship.</summary>
    [HarmonyPostfix]
    private static void AnimalHouseAddNewHatchedAnimalPostfix(AnimalHouse __instance)
    {
        var who = Game1.getFarmer(__instance.getBuilding().owner.Value);
        if (!who.HasProfession(Profession.Rancher)) return;

        var a = __instance.Animals?.Values.Last();
        if (a is null || a.age.Value != 0 || a.friendshipTowardFarmer.Value != 0) return;
        a.friendshipTowardFarmer.Value =
            200 + new Random(__instance.GetHashCode() + a.GetHashCode()).Next(-50, 51);
    }

    #endregion harmony patches
}