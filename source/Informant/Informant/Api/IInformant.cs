/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.TerrainFeatures;

namespace Slothsoft.Informant.Api; 

/// <summary>
/// Base class for the entire API. Can be used to add custom information providers.
/// </summary>
public interface IInformant {

    /// <summary>
    /// A manager class for the <see cref="TerrainFeature"/>(s) under the mouse position.
    /// </summary>
    ITooltipGeneratorManager<TerrainFeature> TerrainFeatureTooltipGenerators { get; }

    /// <summary>
    /// Adds a tooltip generator for the <see cref="TerrainFeature"/>(s) under the mouse position.
    /// <br/><b>Since Version:</b> 1.1.0
    /// </summary>
    [Obsolete("This method is deprecated since it doesn't support localization, use the overlay with Func instead")]
    void AddTerrainFeatureTooltipGenerator(string id, string displayName, string description, Func<TerrainFeature, string> generator); 
    /// <summary>
    /// Adds a tooltip generator for the <see cref="TerrainFeature"/>(s) under the mouse position.
    /// <br/><b>Since Version:</b> 1.3.0
    /// </summary>
    void AddTerrainFeatureTooltipGenerator(string id, Func<string> displayName, Func<string> description, Func<TerrainFeature, string> generator); 
    
    /// <summary>
    /// A manager class for the <see cref="SObject"/>(s) under the mouse position.
    /// </summary>
    ITooltipGeneratorManager<SObject> ObjectTooltipGenerators { get; }
    
    /// <summary>
    /// Adds a tooltip generator for the <see cref="Object"/>(s) under the mouse position.
    /// <br/><b>Since Version:</b> 1.1.0
    /// </summary>
    [Obsolete("This method is deprecated since it doesn't support localization, use the overlay with Func instead")]
    void AddObjectTooltipGenerator(string id, string displayName, string description, Func<SObject, string?> generator); 
    /// <summary>
    /// Adds a tooltip generator for the <see cref="Object"/>(s) under the mouse position.
    /// <br/><b>Since Version:</b> 1.3.0
    /// </summary>
    void AddObjectTooltipGenerator(string id, Func<string> displayName, Func<string> description, Func<SObject, string?> generator); 
    
    /// <summary>
    /// A manager class for decorating a tooltip for an <see cref="Item"/>.
    /// </summary>
    IDecoratorManager<Item> ItemDecorators { get; }
    
    /// <summary>
    /// Adds a decorator for the <see cref="Item"/>(s) under the mouse position.
    /// <br/><b>Since Version:</b> 1.1.0
    /// </summary>
    [Obsolete("This method is deprecated since it doesn't support localization, use the overlay with Func instead")]
    void AddItemDecorator(string id, string displayName, string description, Func<Item, Texture2D?> decorator); 
    /// <summary>
    /// Adds a decorator for the <see cref="Item"/>(s) under the mouse position.
    /// <br/><b>Since Version:</b> 1.3.0
    /// </summary>
    void AddItemDecorator(string id, Func<string> displayName, Func<string> description, Func<Item, Texture2D?> decorator); 
    
    /// <summary>
    /// A list of other classes that add information somwhere in the game.
    /// <br/><b>Since Version:</b> 1.2.1
    /// </summary>
    IEnumerable<IDisplayable> GeneralDisplayables { get; }
}