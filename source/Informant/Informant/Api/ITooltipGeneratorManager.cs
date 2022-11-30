/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System.Collections.Generic;

namespace Slothsoft.Informant.Api; 

/// <summary>
/// A general manager that allows hooking new providers for a specific type into.
/// </summary>
public interface ITooltipGeneratorManager<TInput> {

    /// <summary>
    /// Returns the generators this manager has.
    /// </summary>
    IEnumerable<IDisplayable> Generators { get; }

    /// <summary>
    /// Add a generator that provides information for a specific type.
    /// </summary>
    /// <param name="generator">the generator to add.</param>
    void Add(ITooltipGenerator<TInput> generator);
    
    /// <summary>
    /// Removes a generator that provides information for a specific type.
    /// </summary>
    /// <param name="generatorId">the generator's ID to remove.</param>
    void Remove(string generatorId);
}