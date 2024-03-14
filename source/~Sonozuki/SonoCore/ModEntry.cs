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

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    /*********
    ** Properties
    *********/
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>The singleton instance of <see cref="ModEntry"/>.</summary>
    public static ModEntry Instance { get; private set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    /*********
    ** Public Methods
    *********/
    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        Instance = this;
    }
}
