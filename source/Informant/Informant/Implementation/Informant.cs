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
using Slothsoft.Informant.Api;
using Slothsoft.Informant.Implementation.Decorator;
using Slothsoft.Informant.Implementation.TooltipGenerator;
using StardewValley.TerrainFeatures;

namespace Slothsoft.Informant.Implementation; 

public class Informant : IInformant {

    private readonly IModHelper _modHelper;
    
    private TooltipGeneratorManager? _terrainFeatureInformant;
    private ItemDecoratorManager? _itemDecoratorInformant;
    private readonly SellPriceDisplayable _sellPriceDisplayable;
    private readonly NewRecipeDisplayable _newRecipeDisplayable;
    
    public Informant(IModHelper modHelper) {
        _modHelper = modHelper;
        _sellPriceDisplayable = new SellPriceDisplayable(modHelper);
        _newRecipeDisplayable = new NewRecipeDisplayable(modHelper);
    }
    
    public ITooltipGeneratorManager<TerrainFeature> TerrainFeatureTooltipGenerators {
        get {
            _terrainFeatureInformant ??= new TooltipGeneratorManager(_modHelper);
            return _terrainFeatureInformant;
        }
    }

    public void AddTerrainFeatureTooltipGenerator(string id, string displayName, string description, Func<TerrainFeature, string> generator) {
        TerrainFeatureTooltipGenerators.Add(new TooltipGenerator<TerrainFeature>(id, () => displayName, () => description, generator));
    }
    
    public void AddTerrainFeatureTooltipGenerator(string id, Func<string> displayName, Func<string> description, Func<TerrainFeature, string> generator) {
        TerrainFeatureTooltipGenerators.Add(new TooltipGenerator<TerrainFeature>(id, displayName, description, generator));
    }

    public ITooltipGeneratorManager<SObject> ObjectTooltipGenerators {
        get {
            _terrainFeatureInformant ??= new TooltipGeneratorManager(_modHelper);
            return _terrainFeatureInformant;
        }
    }

    public void AddObjectTooltipGenerator(string id, string displayName, string description, Func<SObject, string?> generator) {
        ObjectTooltipGenerators.Add(new TooltipGenerator<SObject>(id, () => displayName, () => description, generator));
    }
    
    public void AddObjectTooltipGenerator(string id, Func<string> displayName, Func<string> description, Func<SObject, string?> generator) {
        ObjectTooltipGenerators.Add(new TooltipGenerator<SObject>(id, displayName, description, generator));
    }
    
    public IDecoratorManager<Item> ItemDecorators {
        get {
            _itemDecoratorInformant ??= new ItemDecoratorManager(_modHelper);
            return _itemDecoratorInformant;
        }
    }
    
    public void AddItemDecorator(string id, string displayName, string description, Func<Item, Texture2D?> decorator) {
        ItemDecorators.Add(new Decorator<Item>(id, () => displayName, () => description, decorator));
    }
    
    public void AddItemDecorator(string id, Func<string> displayName, Func<string> description, Func<Item, Texture2D?> decorator) {
        ItemDecorators.Add(new Decorator<Item>(id, displayName, description, decorator));
    }

    public IEnumerable<IDisplayable> GeneralDisplayables => new IDisplayable[] {_sellPriceDisplayable, _newRecipeDisplayable};
}