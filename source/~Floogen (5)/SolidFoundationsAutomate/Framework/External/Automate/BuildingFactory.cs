/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using SolidFoundations.Framework.Models.ContentPack;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundationsAutomate.Framework.External.Automate
{
    internal class BuildingFactory : IAutomationFactory
    {
        public IAutomatable GetFor(StardewValley.Object obj, GameLocation location, in Vector2 tile)
        {
            return null;
        }

        public IAutomatable GetFor(TerrainFeature feature, GameLocation location, in Vector2 tile)
        {
            return null;
        }

        public IAutomatable GetFor(Building building, BuildableGameLocation location, in Vector2 tile)
        {
            if (building is GenericBuilding customBuilding && customBuilding.Model is not null && customBuilding.Model.DisableAutomate is false)
            {
                return new BuildingMachine(customBuilding, location, tile);
            }

            return null;
        }

        public IAutomatable GetForTile(GameLocation location, in Vector2 tile)
        {
            return null;
        }
    }
}
