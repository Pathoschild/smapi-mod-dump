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

namespace EscasModdingPlugins
{
    /// <summary>API implementation. Allows direct interaction with other SMAPI mods, e.g. access to EMP feature data.</summary>
    /// <remarks>Refer to <see cref="IEmpApi"/> for usage documentation.</remarks>
    public class EmpApi
    {
        public EmpApi()
        {
        }

        public void GetFishLocationsData(GameLocation location, Vector2 tile, out string useLocationName, out int? useZone, out bool? useOceanCrabPots)
        {
            //get fishing data for this tile
            var data = TileData.GetDataForTile<FishLocationsData>(HarmonyPatch_FishLocations.AssetName, HarmonyPatch_FishLocations.TilePropertyName, location, (int)tile.X, (int)tile.Y);

            //output any fish location settings that exist for this tile
            useLocationName = data?.UseLocation ?? null;
            useZone = data?.UseZone ?? null;
            useOceanCrabPots = data?.UseOceanCrabPots ?? null;
        }
    }
}
