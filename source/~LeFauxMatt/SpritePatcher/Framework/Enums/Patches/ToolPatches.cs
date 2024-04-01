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
using StardewValley.Tools;

/// <summary>Represents the classes of each tool object that is supported by this mod.</summary>
[EnumExtensions]
public enum ToolPatches
{
    /// <summary>An object of type <see cref="FishingRod" />.</summary>
    PatchedFishingRod,

    /// <summary>An object of type <see cref="MeleeWeapon" />.</summary>
    PatchedMeleeWeapon,

    /// <summary>An object of type <see cref="Slingshot" />.</summary>
    PatchedSlingshot,

    /// <summary>An object of type <see cref="WateringCan" />.</summary>
    PatchedWateringCan,
}