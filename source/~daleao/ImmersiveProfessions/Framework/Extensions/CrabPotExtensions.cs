/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

using ObjectLookups = Utility.ObjectLookups;

#endregion using directives

/// <summary>Extensions for the <see cref="CrabPot"/> class.</summary>
public static class CrabPotExtensions
{
    /// <summary>Whether the crab pot instance is using magnet as bait.</summary>
    public static bool HasMagnet(this CrabPot crabpot)
    {
        return crabpot.bait.Value is not null &&
               crabpot.bait.Value.ParentSheetIndex == 703;
    }

    /// <summary>Whether the crab pot instance is using wild bait.</summary>
    public static bool HasWildBait(this CrabPot crabpot)
    {
        return crabpot.bait.Value is not null &&
               crabpot.bait.Value.ParentSheetIndex == 774;
    }

    /// <summary>Whether the crab pot instance is using magic bait.</summary>
    public static bool HasMagicBait(this CrabPot crabpot)
    {
        return crabpot.bait.Value is not null &&
               crabpot.bait.Value.ParentSheetIndex == 908;
    }

    /// <summary>Whether the crab pot instance should catch ocean-specific shellfish.</summary>
    /// <param name="location">The location of the crab pot.</param>
    public static bool ShouldCatchOceanFish(this CrabPot crabpot, GameLocation location)
    {
        return location is Beach ||
               location.catchOceanCrabPotFishFromThisSpot((int) crabpot.TileLocation.X,
                   (int) crabpot.TileLocation.Y);
    }

    /// <summary>Whether the given crab pot instance is holding an object that can only be caught via Luremaster profession.</summary>
    public static bool HasSpecialLuremasterCatch(this CrabPot crabpot)
    {
        var obj = crabpot.heldObject.Value;
        return obj is not null && (obj.IsFish() && !obj.IsTrapFish() || obj.IsAlgae() || obj.IsPirateTreasure());
    }
}