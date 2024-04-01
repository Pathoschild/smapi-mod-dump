/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Text;
using Microsoft.Build.Framework;

namespace TerrainFeatureRefresh.Framework;

public struct TfrSettings
{
    // TODO for later: Store the predicate in each individual TfrFeature in order to tidy up FeatureProcessor.cs.

    public TfrToggle AffectAllLocations = new TfrToggle();

    // Objects
    public TfrFeature Fences = new TfrFeature();
    public TfrFeature Weeds = new TfrFeature();
    public TfrFeature Twigs = new TfrFeature();
    public TfrFeature Stones = new TfrFeature();
    public TfrFeature Forage = new TfrFeature();
    public TfrFeature ArtifactSpots = new TfrFeature();

    // Terrain Features
    public TfrFeature Grass = new TfrFeature();
    public TfrFeature WildTrees = new TfrFeature();
    public TfrFeature FruitTrees = new TfrFeature();
    public TfrFeature Paths = new TfrFeature();
    public TfrFeature HoeDirt = new TfrFeature();
    public TfrFeature Crops = new TfrFeature();
    public TfrFeature Bushes = new TfrFeature();

    // Resource Clumps
    public TfrFeature Stumps = new TfrFeature();
    public TfrFeature Logs = new TfrFeature();
    public TfrFeature Boulders = new TfrFeature();
    public TfrFeature Meteorites = new TfrFeature();

    public TfrSettings() { }

    public override string ToString()
    {
        StringBuilder returned = new StringBuilder();
        returned.AppendLine($"Affect all locations: {this.AffectAllLocations}");

        returned.AppendLine($"SObjects");
        returned.AppendLine($"=========================");
        returned.AppendLine($"Fences: {this.Fences.ToString()}");
        returned.AppendLine($"Weeds: {this.Weeds.ToString()}");
        returned.AppendLine($"Twigs: {this.Twigs.ToString()}");
        returned.AppendLine($"Stones: {this.Stones.ToString()}");
        returned.AppendLine($"Forage: {this.Forage.ToString()}");
        returned.AppendLine($"Artifact Spots: {this.ArtifactSpots.ToString()}");
        returned.AppendLine("\n");

        returned.AppendLine($"TerrainFeatures");
        returned.AppendLine($"=========================");
        returned.AppendLine($"Grass: {this.Grass.ToString()}");
        returned.AppendLine($"Wild trees: {this.WildTrees.ToString()}");
        returned.AppendLine($"Fruit trees: {this.FruitTrees.ToString()}");
        returned.AppendLine($"Paths: {this.Paths.ToString()}");
        returned.AppendLine($"Hoe dirt: {this.HoeDirt.ToString()}");
        returned.AppendLine($"Crops: {this.Crops.ToString()}");
        returned.AppendLine($"Bushes: {this.Bushes.ToString()}");
        returned.AppendLine("\n");

        returned.AppendLine($"ResourceClumps");
        returned.AppendLine($"=========================");
        returned.AppendLine($"Stumps: {this.Stumps.ToString()}");
        returned.AppendLine($"Logs: {this.Logs.ToString()}");
        returned.AppendLine($"Boulders: {this.Boulders.ToString()}");
        returned.AppendLine($"Meteorites: {this.Meteorites.ToString()}");

        return returned.ToString();
    }
}
