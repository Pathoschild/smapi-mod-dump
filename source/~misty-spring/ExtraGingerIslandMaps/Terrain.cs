/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace ExtraGingerIslandMaps
{
    internal static class Terrain
    {
        internal static List<Type> TypesToReset = new() 
        {
            typeof(Bush),
            typeof(Tree),
            typeof(Grass),
            //typeof(ResourceClump),
            typeof(TerrainFeature)
        };

        internal static void Reset(GameLocation where)
        {
			Clear(where);
			Restart(where);
        }

		private static void Clear(GameLocation where)
		{
            List<TerrainFeature> currentFeatures = new();
            List<LargeTerrainFeature> largeTerrains = new();

            //include _active
            foreach (var active in where._activeTerrainFeatures)
            {
                //if not specific type we want to erase
                if(TypesToReset.Contains(active.GetType()) == false)
                {
                    continue;
                }

                currentFeatures.Add(active);
            }
            //add large terrain features
            foreach (LargeTerrainFeature largeTerrainFeature in where.largeTerrainFeatures)
            {
                if (TypesToReset.Contains(((object)largeTerrainFeature).GetType()))
                {
                    largeTerrains.Add(largeTerrainFeature);
                }
            }

            //remove terrain features
            foreach(var type in currentFeatures)
            {
                Vector2 key = new(
                    (int)type.currentTileLocation.X, 
                    (int)type.currentTileLocation.Y
                    );

                if (where.terrainFeatures.ContainsKey(key))
                {
                    where.terrainFeatures.Remove(key);
                }
            }

            foreach(var largetype in largeTerrains)
            {
                if (where.largeTerrainFeatures.Contains(largetype))
                {
                    where.largeTerrainFeatures.Remove(largetype);
                }
            }
        }

        private static void Restart(GameLocation where)
        {
            where.reloadMap();
            where.seasonUpdate("summer", false);
            where.loadObjects();
            //Utility.clearObjectsInArea(new Rectangle(0, 0, where.Map.DisplayWidth, where.Map.DisplayHeight), where);
        }
    }
}