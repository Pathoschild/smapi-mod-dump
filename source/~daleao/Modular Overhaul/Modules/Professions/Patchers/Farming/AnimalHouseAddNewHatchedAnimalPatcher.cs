/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Farming;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class AnimalHouseAddNewHatchedAnimalPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AnimalHouseAddNewHatchedAnimalPatcher"/> class.</summary>
    internal AnimalHouseAddNewHatchedAnimalPatcher()
    {
        this.Target = this.RequireMethod<AnimalHouse>(nameof(AnimalHouse.addNewHatchedAnimal));
    }

    #region harmony patches

    /// <summary>Patch for Rancher newborn animals to have random starting friendship.</summary>
    [HarmonyPostfix]
    private static void AnimalHouseAddNewHatchedAnimalPostfix(AnimalHouse __instance)
    {
        var owner = Game1.getFarmer(__instance.getBuilding().owner.Value);
        if (!owner.HasProfession(Profession.Rancher) &&
            (!ProfessionsModule.Config.LaxOwnershipRequirements ||
             !Game1.game1.DoesAnyPlayerHaveProfession(Profession.Rancher, out _)))
        {
            return;
        }

        var newborn = __instance.Animals.Values.Last();
        if (newborn is null || newborn.age.Value != 0 || newborn.friendshipTowardFarmer.Value != 0 ||
            (newborn.ownerID.Value != owner.UniqueMultiplayerID && !ProfessionsModule.Config.LaxOwnershipRequirements))
        {
            return;
        }

        newborn.friendshipTowardFarmer.Value =
            200 + new Random(__instance.GetHashCode() + newborn.GetHashCode()).Next(-50, 51);
    }

    #endregion harmony patches
}
