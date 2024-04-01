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
using StardewValley.Characters;

/// <summary>Represents the classes of each character object that is supported by this mod.</summary>
[EnumExtensions]
internal enum CharacterPatches
{
    /// <summary>An object of type <see cref="AnimatedSprite" />.</summary>
    PatchedAnimatedSprite,

    /// <summary>An object of type <see cref="Child" />.</summary>
    PatchedChild,

    /// <summary>An object of type <see cref="FarmAnimal" />.</summary>
    PatchedFarmAnimal,

    /// <summary>An object of type <see cref="Horse" />.</summary>
    PatchedHorse,

    /// <summary>An object of type <see cref="JunimoHarvester" />.</summary>
    PatchedJunimoHarvester,

    /// <summary>An object of type <see cref="Junimo" />.</summary>
    PatchedJunimo,

    /// <summary>An object of type <see cref="Pet" />.</summary>
    PatchedPet,
}