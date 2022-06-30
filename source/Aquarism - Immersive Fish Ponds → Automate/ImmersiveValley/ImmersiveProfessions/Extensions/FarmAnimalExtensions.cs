/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using StardewValley;
using System;

#endregion

/// <summary>Extensions for the <see cref="FarmAnimal"/> class.</summary>
public static class FarmAnimalExtensions
{
    /// <summary>Affects the price of animals sold by Breeder.</summary>
    public static double GetProducerAdjustedFriendship(this FarmAnimal animal)
    {
        return Math.Pow(Math.Sqrt(2) * animal.friendshipTowardFarmer.Value / 1000, 2) + 0.5;
    }
}