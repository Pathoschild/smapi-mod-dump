/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using DynamicGameAssets.Game;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace DGAAutomate
{
    internal class MyAutomationFactory : IAutomationFactory
    {
        public IAutomatable GetFor(StardewValley.Object obj, GameLocation location, in Vector2 tile)
        {
            if (obj is CustomBigCraftable cbig)
            {
                if (DynamicGameAssets.Mod.customMachineRecipes.ContainsKey(cbig.FullId))
                    return new MyMachine(cbig, location, tile);
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
