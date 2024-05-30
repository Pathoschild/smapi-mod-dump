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
namespace StardewMods.FauxCore.Common.Services.Integrations.BetterChests;
#else
namespace StardewMods.Common.Services.Integrations.BetterChests;
#endif

using NetEscapades.EnumGenerators;

/// <summary>The possible values for Chest capacity.</summary>
[EnumExtensions]
public enum ChestMenuOption
{
    /// <summary>Capacity is inherited by a parent config.</summary>
    Default = 0,

    /// <summary>Vanilla slots.</summary>
    Disabled = 1,

    /// <summary>Resize to 9 item slots.</summary>
    Small = 9,

    /// <summary>Resize to 36 item slots.</summary>
    Medium = 36,

    /// <summary>Resize to 70 item slots.</summary>
    Large = 70,
}