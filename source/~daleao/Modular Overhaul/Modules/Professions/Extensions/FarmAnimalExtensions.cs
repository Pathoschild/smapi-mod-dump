/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Extensions;

#region using directives

using DaLion.Shared.Extensions.Stardew;

#endregion

/// <summary>Extensions for the <see cref="FarmAnimal"/> class.</summary>
internal static class FarmAnimalExtensions
{
    /// <summary>Determines whether the owner of the <paramref name="animal"/> has the specified <paramref name="profession"/>.</summary>
    /// <param name="animal">The <see cref="FarmAnimal"/>.</param>
    /// <param name="profession">An <see cref="IProfession"/>..</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <see cref="Farmer"/> who owns the <paramref name="animal"/> has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool DoesOwnerHaveProfession(this FarmAnimal animal, IProfession profession, bool prestiged = false)
    {
        return animal.GetOwner().HasProfession(profession, prestiged);
    }

    /// <summary>Adjusts the price of the <paramref name="animal"/> for <see cref="Profession.Breeder"/>.</summary>
    /// <param name="animal">The <see cref="FarmAnimal"/>.</param>
    /// <returns>The adjusted friendship value.</returns>
    internal static double GetProducerAdjustedFriendship(this FarmAnimal animal)
    {
        return Math.Pow(Math.Sqrt(2) * animal.friendshipTowardFarmer.Value / 1000, 2) + 0.5;
    }
}
