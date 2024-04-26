/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/EscasModdingPlugins
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace EscasModdingPlugins
{
    /// <summary>API implementation. Allows direct interaction with other SMAPI mods, e.g. access to EMP feature data.</summary>
    /// <remarks>Refer to <see cref="IEmpApi"/> for usage documentation.</remarks>
    public class EmpApi : IEmpApi
    {
        public EmpApi()
        {
        }

        /// <inheritdoc/>
        [Obsolete("In EMP v1.2.3 and later, some of these parameters are disabled or implemented differently. Please use another overload of this method.", false)]
        public void GetFishLocationsData(GameLocation location, Vector2 tile, out string useLocationName, out int? useZone, out bool? useOceanCrabPots)
        {
            //get fishing data for this tile
            var data = TileData.GetDataForTile<FishLocationsData>(HarmonyPatch_FishLocations.AssetName, HarmonyPatch_FishLocations.TilePropertyName, location, (int)tile.X, (int)tile.Y);

            //output any fish location settings that exist for this tile
            useLocationName = data?.UseLocation ?? null;
            useZone = null; //disabled
            useOceanCrabPots = data?.UseOceanCrabPots ?? null; //obsolete and overridden by a new field, but still functional in legacy mods
        }

        /// <inheritdoc/>
        public void GetFishLocationsData(GameLocation location, Vector2 tile, out string useLocationName, out Vector2? useTile, out List<string> useCrabPotTypes)
        {
            //get fishing data for this tile
            var data = TileData.GetDataForTile<FishLocationsData>(HarmonyPatch_FishLocations.AssetName, HarmonyPatch_FishLocations.TilePropertyName, location, (int)tile.X, (int)tile.Y);

            useLocationName = data?.UseLocation ?? null;
            useTile = data?.UseTile?.AsVector2() ?? null;
            useCrabPotTypes = data?.UseCrabPotTypes ?? null;
        }
    }
}
