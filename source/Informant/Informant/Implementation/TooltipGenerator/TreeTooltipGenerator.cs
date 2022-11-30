/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using Slothsoft.Informant.Api;
using StardewValley.TerrainFeatures;

namespace Slothsoft.Informant.Implementation.TooltipGenerator;

internal class TreeTooltipGenerator : ITooltipGenerator<TerrainFeature> {

    private readonly IModHelper _modHelper;
    
    public TreeTooltipGenerator(IModHelper modHelper) {
        _modHelper = modHelper;
    }
    
    public string Id => "tree";
    public string DisplayName => _modHelper.Translation.Get("TreeTooltipGenerator");
    public string Description => _modHelper.Translation.Get("TreeTooltipGenerator.Description");
    
    public bool HasTooltip(TerrainFeature input) {
        return input is Tree;
    }

    public Tooltip Generate(TerrainFeature input) {
        return new Tooltip(CreateText((Tree) input));
    }

    private string CreateText(Tree tree) {
        switch (tree.treeType.Value) {
            case Tree.bushyTree:
            case Tree.leafyTree:
            case Tree.pineTree:
            case Tree.palmTree:
            case Tree.mushroomTree:
            case Tree.mahoganyTree:
            case Tree.palmTree2:
                return _modHelper.Translation.Get("TreeTooltipGenerator.Type" + tree.treeType.Value);
        }
        return "???";
    }
}