/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Extensions.Stardew;

/// <summary>Extensions for the <see cref="FarmAnimal"/> class.</summary>
public static class FarmAnimalExtensions
{
    /// <summary>Gets the <see cref="Farmer"/> instance who owns this <paramref name="animal"/>.</summary>
    /// <param name="animal">The <see cref="FarmAnimal"/>.</param>
    /// <returns>The <see cref="Farmer"/> instance who purchased or owned the parent of the <paramref name="animal"/>, or the host of the game session if not found.</returns>
    public static Farmer GetOwner(this FarmAnimal animal)
    {
        return Game1.getFarmerMaybeOffline(animal.ownerID.Value) ?? Game1.MasterPlayer;
    }
}
