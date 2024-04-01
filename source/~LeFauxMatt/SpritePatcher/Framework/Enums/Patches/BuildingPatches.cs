/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.SpritePatcher.Framework.Enums.Patches;

using NetEscapades.EnumGenerators;
using StardewValley.Buildings;

/// <summary>Represents the classes of each building object that is supported by this mod.</summary>
[EnumExtensions]
public enum BuildingPatches
{
    /// <summary>An object of type <see cref="Building" />.</summary>
    PatchedBuilding,

    /// <summary>An object of type <see cref="FishPond" />.</summary>
    PatchedFishPond,

    /// <summary>An object of type <see cref="JunimoHut" />.</summary>
    PatchedJunimoHut,

    /// <summary>An object of type <see cref="PetBowl" />.</summary>
    PatchedPetBowl,

    /// <summary>An object of type <see cref="ShippingBin" />.</summary>
    PatchedShippingBin,
}