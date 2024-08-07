/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;

namespace LeaderboardLibrary;


public sealed class GlobalModData
{
    public string UserUuid { get; init; } = Guid.NewGuid().ToString();
    public string Secret { get; init; } = Guid.NewGuid().ToString();
}
