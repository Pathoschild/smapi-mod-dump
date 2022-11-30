/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using Slothsoft.Informant.ThirdParty;

namespace Slothsoft.Informant.Api; 

/// <summary>
/// A class that can be registered in the <see cref="IGenericModConfigMenuApi"/>.
/// </summary>
public interface IDisplayable {

    /// <summary>
    /// The unique ID of the generator. 
    /// </summary>
    string Id { get; }
    
    /// <summary>
    /// The display name of the generator for the configuration. 
    /// </summary>
    string DisplayName { get; }
    
    /// <summary>
    /// The description of the generator for the configuration. 
    /// </summary>
    string Description { get; }
}