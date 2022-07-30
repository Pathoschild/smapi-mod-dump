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
#pragma warning disable SA1623 // Property summary documentation should match accessors - copied from Automate to use the API.

/// <summary>An ingredient stack (or stacks) which can be consumed by a machine.</summary>
public interface IConsumable
{
    /// <summary>The items available to consumable.</summary>
    ITrackedStack Consumables { get; }

    /// <summary>A sample item for comparison.</summary>
    /// <remarks>This should not be a reference to the original stack.</remarks>
    Item Sample { get; }

    /// <summary>The number of items needed for the recipe.</summary>
    int CountNeeded { get; }

    /// <summary>Whether the consumables needed for this requirement are ready.</summary>
    bool IsMet { get; }

    /// <summary>Remove the needed number of this item from the stack.</summary>
    void Reduce();

    /// <summary>Remove the needed number of this item from the stack and return a new stack matching the count.</summary>
    Item? Take();
}

#pragma warning restore SA1623 // Property summary documentation should match accessors