using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace PFMAutomate.Automate
{
    internal class ProducerFrameworkAutomationFactory : IAutomationFactory
    {
        public IAutomatable GetFor(SObject obj, GameLocation location, in Vector2 tile)
        {
            if (ProducerFrameworkMod.ProducerController.HasProducerRule(obj.Name))
            {
                return new CustomProducerMachine(obj,location,tile);
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
