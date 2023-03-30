/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Xebeth/StardewValley-ListModsCommand
**
*************************************************/

﻿using StardewModdingAPI;
using System.Collections.Immutable;

namespace ModListCommand;

public sealed record ModInfo
{
    public required string Name { get; init; }
    public required ISemanticVersion Version { get; init; }
    public required string Author { get; init; }
    public required string Description { get; init; }
    public required ImmutableList<string> UpdateUrls { get; init; }
}
