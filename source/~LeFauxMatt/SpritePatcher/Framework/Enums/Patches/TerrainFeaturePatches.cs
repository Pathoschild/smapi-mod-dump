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
using StardewValley.TerrainFeatures;

/// <summary>Represents the classes of each terrain feature object that is supported by this mod.</summary>
[EnumExtensions]
public enum TerrainFeaturePatches
{
    /// <summary>An object of type <see cref="Bush" />.</summary>
    PatchedBush,

    /// <summary>An object of type <see cref="CosmeticPlant" />.</summary>
    PatchedCosmeticPlant,

    /// <summary>An object of type <see cref="Crop" />.</summary>
    PatchedCrop,

    /// <summary>An object of type <see cref="Flooring" />.</summary>
    PatchedFlooring,

    /// <summary>An object of type <see cref="FruitTree" />.</summary>
    PatchedFruitTree,

    /// <summary>An object of type <see cref="GiantCrop" />.</summary>
    PatchedGiantCrop,

    /// <summary>An object of type <see cref="Grass" />.</summary>
    PatchedGrass,

    /// <summary>An object of type <see cref="HoeDirt" />.</summary>
    PatchedHoeDirt,

    /// <summary>An object of type <see cref="ResourceClump" />.</summary>
    PatchedResourceClump,

    /// <summary>An object of type <see cref="Tree" />.</summary>
    PatchedTree,
}