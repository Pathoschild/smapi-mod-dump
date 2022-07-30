/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.Integrations.Interfaces.Automate;

/// <summary>
/// A recipe for Automate.
/// </summary>
public interface IRecipe
{
    /// <summary>Gets a function that matches items that can be used as input.</summary>
    Func<Item, bool> Input { get; }

    /// <summary>Gets the number of inputs needed.</summary>
    int InputCount { get; }

    /// <summary>Gets a function that maches the output to generate (given an input).</summary>
    Func<Item, Item> Output { get; }

    /// <summary>Gets a function that get the time needed to prepare an output (given an input).</summary>
    Func<Item, int> Minutes { get; }

    /// <summary>Get whether the recipe can accept a given item as input (regardless of stack size).</summary>
    /// <param name="stack">The item to check.</param>
    bool AcceptsInput(ITrackedStack stack);
}
