using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using AutoQualityPatch.Automatables;
using QualityProducts;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace AutoQualityPatch
{
    internal class ProcessorFactory : IAutomationFactory
    {
        public IAutomatable GetFor(SObject obj, GameLocation location, in Vector2 tile)
        {
            if (obj.name.EndsWith(Processor.ProcessorNameSuffix, System.StringComparison.Ordinal))
            {
                switch (obj.name.Remove(obj.Name.Length - Processor.ProcessorNameSuffix.Length))
                {
                    case "Keg":
                        return new AutomatableKeg(obj, location, tile);
                    case "Preserves Jar":
                        return new AutomatablePreservesJar(obj, location, tile);
                    case "Cheese Press":
                        return new AutomatableCheesePress(obj, location, tile);
                    case "Loom":
                        return new AutomatableLoom(obj, location, tile);
                    case "Oil Maker":
                        return new AutomatableOilMaker(obj, location, tile);
                    case "Mayonnaise Machine":
                        return new AutomatableMayonnaiseMachine(obj, location, tile);
                }
            }

            return null;
        }

        public IAutomatable GetFor(TerrainFeature feature, GameLocation location, in Vector2 tile)
        {
            return null;
        }

        public IAutomatable GetFor(Building building, BuildableGameLocation location, in Vector2 tile)
        {
            return null;
        }

        public IAutomatable GetForTile(GameLocation location, in Vector2 tile)
        {
            return null;
        }
    }
}