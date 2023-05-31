/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.Constants.Ids.Objects;
using Omegasis.Revitalize.Framework.World.Objects;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.Revitalize.Framework.World.Objects.Machines;
using Omegasis.Revitalize.Framework.World.Objects.Machines.Furnaces;
using Omegasis.Revitalize.Framework.World.Objects.Machines.Misc;
using Omegasis.Revitalize.Framework.World.Objects.Machines.ResourceGeneration;
using Omegasis.Revitalize.Framework.World.Objects.Storage;
using Omegasis.RevitalizeAutomateCompatibility.Objects;
using Omegasis.RevitalizeAutomateCompatibility.Objects.Machines;
using Omegasis.RevitalizeAutomateCompatibility.Objects.Storage;
using Pathoschild.Stardew.Automate;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace Omegasis.RevitalizeAutomateCompatibility
{
    public class RevitalizeAutomationFactory : IAutomationFactory
    {
        /// <summary>Get a machine, container, or connector instance for a given object.</summary>
        /// <param name="obj">The in-game object.</param>
        /// <param name="location">The location to check.</param>
        /// <param name="tile">The tile position to check.</param>
        /// <returns>Returns an instance or <c>null</c>.</returns>
        public IAutomatable GetFor(StardewValley.Object obj, GameLocation location, in Vector2 tile)
        {
            if (obj is ICustomModObject)
            {
                ICustomModObject modObj = (obj as ICustomModObject);

                //Can add more specific wrapper types here where we check by modObj.Id, but the below cases should catch the majority of the logic.

                if (modObj.Id.Equals(FarmingObjectIds.IrrigatedGardenPot))
                {
                    //Irrigated garden pots are disabled for automate compatibility since they are balanaced out with the automatic farming system and it's attachments.
                    return null;
                }

                //Add in item vaults as a type of storage accessor.
                if (obj is ItemVault)
                {
                    return new ItemVaultWrapper((ItemVault)obj, location, tile);
                }

                if (obj is DimensionalStorageChest)
                {
                    return new DimensionalStorageChestWrapper((DimensionalStorageChest)obj, location, tile);
                }

                //Add in generic processing wrapper types.
                if (obj is PoweredMachine)
                {
                    return new PoweredMachineWrapper<PoweredMachine>((PoweredMachine)obj, location, tile);
                }

                //Add in wrapper type for machines if they are not powered machines.
                if (obj is Machine && !(obj is PoweredMachine))
                {
                    return new MachineWrapper<Machine>((Machine)obj, location, tile);
                }

                //Add in wrapper for generic custom objects.
                if(obj is CustomObject && !(obj is PoweredMachine) && !(obj is Machine))
                {
                    return new CustomObjectWrapper<CustomObject>((CustomObject)obj, location, tile);
                }

            }

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
