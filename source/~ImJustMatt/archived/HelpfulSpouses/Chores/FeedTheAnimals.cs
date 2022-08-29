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

internal class FeedTheAnimals : IChore
{
    private static FeedTheAnimals? Instance;

    private readonly IModHelper _helper;

    private FeedTheAnimals(IModHelper helper)
    {
        this._helper = helper;
    }

    /// <inheritdoc/>
    public bool IsPossible { get; }

    /// <summary>
    ///     Initializes <see cref="FeedTheAnimals" />.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <returns>Returns an instance of the <see cref="FeedTheAnimals" /> class.</returns>
    public static FeedTheAnimals Init(IModHelper helper)
    {
        return FeedTheAnimals.Instance ??= new(helper);
    }

    /// <inheritdoc/>
    public bool TryToDo(NPC spouse)
    {
        throw new System.NotImplementedException();
    }
}