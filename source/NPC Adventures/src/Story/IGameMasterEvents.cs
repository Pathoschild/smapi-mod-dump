/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using StardewValley;
using System;

namespace NpcAdventure.Story
{
    public interface IGameMasterEvents
    {
        event EventHandler<ICheckEventEventArgs> CheckEvent;
    }

    public interface ICheckEventEventArgs
    {
        GameLocation Location { get; }
        Farmer Player { get; }
    }
}
