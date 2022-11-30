/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

namespace Slothsoft.Informant.Api; 

/// <summary>
/// A class that decorates a vanilla tooltip using <see cref="Decoration"/>.
/// </summary>
/// <typeparam name="TInput">input object.</typeparam>
public interface IDecorator<in TInput> : IDisplayable {

    /// <summary>
    /// Returns true if <see cref="Decorate"/> should be called on this object. 
    /// </summary>
    bool HasDecoration(TInput input);
    
    /// <summary>
    /// Generates the decorator for this object.
    /// </summary>
    Decoration Decorate(TInput input);
}