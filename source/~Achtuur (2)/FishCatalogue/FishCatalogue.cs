/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore;
using AchtuurCore.Events;
using FishCatalogue.Data;
using FishCatalogue.Parsing;
using StardewValley;
using StardewValley.GameData.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishCatalogue;
internal class FishCatalogue
{
    public static Dictionary<string, SpawnConditions> AllFishSpawnConditions;
    /// <summary>
    /// key is fish item qualified id
    /// </summary>
    public static Dictionary<string, FishData> AllFishData;
    public static Dictionary<string, List<FishData>> LocationFishData;
    public static Dictionary<string, List<FishArea>> LocationFishAreas;

    public FishCatalogue()
    {
        LoadFishData();
        LoadLocationData();
    }

    /// <summary>
    /// Returns all fish that the player could catch at the current location
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<FishData> GetCurrentlyAvailableFish()
    {
        return GetCurrentLocationFish().Where(fish_data => fish_data.CanBeCaughtHere());
    }

    public static IEnumerable<FishData> GetCurrentLocationFish()
    {
        if (!LocationFishData.ContainsKey(Game1.currentLocation.Name))
            yield break;

        foreach (FishData fish_data in LocationFishData[Game1.currentLocation.Name])
            yield return fish_data;
    }

    private void LoadFishData()
    {
        if (AllFishSpawnConditions is null)
            AllFishSpawnConditions = new();

        var fish_data = ModEntry.Instance.Helper.GameContent.Load<Dictionary<string, string>>("Data/Fish");
        foreach (string fish_data_line in fish_data.Values)
        {
            SpawnConditions spawn_conditions = SpawnCondtionsFactory.Create(fish_data_line);
            AllFishSpawnConditions[spawn_conditions.FishName] = spawn_conditions;
        }
    }

    private void LoadLocationData()
    {
        LocationFishData = new();
        LocationFishAreas = new();
        AllFishData = new();

        var location_data = ModEntry.Instance.Helper.GameContent.Load<Dictionary<string, LocationData>>("Data/Locations");
        foreach ((string loc, LocationData loc_data) in location_data.Select(x => (x.Key, x.Value)))
        {
            // skip locations without fish
            if (loc_data.Fish.Count <= 0)
                continue;

            if (loc_data.FishAreas.Count > 0)
            {
                List<FishArea> fish_areas = loc_data.FishAreas.Values.Select(d => new FishArea(loc, d)).ToList();
                LocationFishAreas.Add(loc, fish_areas);
            }

            foreach (var fish_data in loc_data.Fish)
            {
                if (!FishData.IsValidFishData(fish_data))
                    continue;

                if (AllFishData.ContainsKey(fish_data.ItemId))
                {
                    AllFishData[fish_data.ItemId].AddLocation(loc);
                }
                else
                {
                    FishData fd = new FishData(loc, fish_data);
                    SpawnConditions spawnConditions = AllFishSpawnConditions.Values
                        .FirstOrDefault(spawn_cond => spawn_cond.FishName == fd.FishItem.Name);

                    fd.AddSpawnConditions(spawnConditions);
                    AllFishData[fish_data.ItemId] = fd;
                }

                if (!LocationFishData.ContainsKey(loc))
                    LocationFishData[loc] = new();
                LocationFishData[loc].Add(AllFishData[fish_data.ItemId]);
            }
        }
    }
}
