using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;
using Pathoschild.Stardew.Automate;
using IndustrialFurnace;

/*
 Made using the template example provided by Pathoschild on
 https://github.com/Pathoschild/StardewMods/tree/develop/Automate

25.3.2020 
*/


namespace IndustrialFurnaceAutomate
{
    public class IndustrialFurnaceAutomationFactory : IAutomationFactory
    {
        private IIndustrialFurnaceAPI industrialFurnaceAPI;


        public IndustrialFurnaceAutomationFactory(IIndustrialFurnaceAPI api)
        {
            this.industrialFurnaceAPI = api;
        }


        /// <summary>Get a machine, container, or connector instance for a given object.</summary>
        /// <param name="obj">The in-game object.</param>
        /// <param name="location">The location to check.</param>
        /// <param name="tile">The tile position to check.</param>
        /// <returns>Returns an instance or <c>null</c>.</returns>
        public IAutomatable GetFor(SObject obj, GameLocation location, in Vector2 tile)
        {
            return null;
        }


        /// <summary>Get a machine, container, or connector instance for a given terrain feature.</summary>
        /// <param name="feature">The terrain feature.</param>
        /// <param name="location">The location to check.</param>
        /// <param name="tile">The tile position to check.</param>
        /// <returns>Returns an instance or <c>null</c>.</returns>
        public IAutomatable GetFor(TerrainFeature feature, GameLocation location, in Vector2 tile)
        {
            return null;
        }


        /// <summary>Get a machine, container, or connector instance for a given building.</summary>
        /// <param name="building">The building.</param>
        /// <param name="location">The location to check.</param>
        /// <param name="tile">The tile position to check.</param>
        /// <returns>Returns an instance or <c>null</c>.</returns>
        public IAutomatable GetFor(Building building, BuildableGameLocation location, in Vector2 tile)
        {
            if (industrialFurnaceAPI.IsBuildingIndustrialFurnace(building))
            {
                IndustrialFurnaceController controller = industrialFurnaceAPI.GetController(building.maxOccupants.Value);

                if (controller != null)
                    return new IndustrialFurnaceMachine(controller, location, tile);
            }
                

            return null;
        }


        /// <summary>Get a machine, container, or connector instance for a given tile position.</summary>
        /// <param name="location">The location to check.</param>
        /// <param name="tile">The tile position to check.</param>
        /// <returns>Returns an instance or <c>null</c>.</returns>
        /// <remarks>Shipping bin logic from <see cref="Farm.leftClick"/>, garbage can logic from <see cref="Town.checkAction"/>.</remarks>
        public IAutomatable GetForTile(GameLocation location, in Vector2 tile)
        {
            return null;
        }
    }
}