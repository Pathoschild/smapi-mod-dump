/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace SonoCore;

/// <summary>The ways that data in a model can be interpreted.</summary>
public enum Action
{
    /// <summary>The data will be added.</summary>
    Add,

    /// <summary>The data will edit previous data.</summary>
    Edit,

    /// <summary>The data will delete previous data.</summary>
    Delete
}
