/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Enums;
#else
namespace StardewMods.Common.Enums;
#endif

using NetEscapades.EnumGenerators;

/// <summary>Represents the quality of an item.</summary>
[EnumExtensions]
public enum ItemQuality
{
    /// <summary>No quality.</summary>
    None = SObject.lowQuality,

    /// <summary>Silver quality.</summary>
    Silver = SObject.medQuality,

    /// <summary>Gold quality.</summary>
    Gold = SObject.highQuality,

    /// <summary>Iridium quality.</summary>
    Iridium = SObject.bestQuality,
}