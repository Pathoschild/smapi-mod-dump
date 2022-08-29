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

/// <summary>
///     Implementation of a Helpful Spouses task.
/// </summary>
internal interface IChore
{
    /// <summary>
    ///     Gets a value indicating whether the chore is possible today.
    /// </summary>
    public bool IsPossible { get; }

    /// <summary>
    ///     Attempts to perform the chore.
    /// </summary>
    /// <returns>Returns true if the chore was performed successfully.</returns>
    public bool TryToDo(NPC spouse);
}