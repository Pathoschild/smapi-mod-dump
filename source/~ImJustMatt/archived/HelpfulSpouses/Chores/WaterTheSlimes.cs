/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.HelpfulSpouses.Chores;

internal class WaterTheSlimes : IChore
{
    private static WaterTheSlimes? Instance;

    private readonly IModHelper _helper;

    private WaterTheSlimes(IModHelper helper)
    {
        this._helper = helper;
    }

    /// <inheritdoc/>
    public bool IsPossible { get; }

    /// <summary>
    ///     Initializes <see cref="WaterTheSlimes" />.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <returns>Returns an instance of the <see cref="WaterTheSlimes" /> class.</returns>
    public static WaterTheSlimes Init(IModHelper helper)
    {
        return WaterTheSlimes.Instance ??= new(helper);
    }

    /// <inheritdoc/>
    public bool TryToDo(NPC spouse)
    {
        throw new System.NotImplementedException();
    }
}