/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

#region using directives

namespace DaLion.Common.Extensions.Stardew;

#endregion using directives

public static class FarmAnimalExtensions
{
    /// <summary>Get the <see cref="Farmer"/> instance that owns this farm animal.</summary>
    public static Farmer GetOwner(this FarmAnimal animal) =>
        Game1.getFarmerMaybeOffline(animal.ownerID.Value) ?? Game1.MasterPlayer;
}