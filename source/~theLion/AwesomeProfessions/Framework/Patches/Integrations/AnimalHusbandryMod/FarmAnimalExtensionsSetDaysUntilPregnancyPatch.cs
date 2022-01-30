/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Integrations;

#region using directives

using System;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Stardew.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class FarmAnimalExtensionsSetDaysUntilBirthPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmAnimalExtensionsSetDaysUntilBirthPatch()
    {
        try
        {
            Original = "AnimalHusbandryMod.animals.FarmAnimalExtensions".ToType()
                .MethodNamed("SetDaysUntilBirth");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to reduce gestation of animals inseminated by Breeder.</summary>
    [HarmonyPrefix]
    private static bool FarmAnimalExtensionsSetDaysUntilPregnancyPrefix(ref FarmAnimal farmAnimal, ref int value)
    {
        var who = Game1.getFarmerMaybeOffline(farmAnimal.ownerID.Value) ?? Game1.MasterPlayer;
        if (!who.IsLocalPlayer || !who.HasProfession(Profession.Breeder)) return true; // run original logic

        value /= who.HasProfession(Profession.Breeder, true) ? 3 : 2;
        value = Math.Max(value, 1);
        return true; // run original logic
    }

    #endregion harmony patches
}