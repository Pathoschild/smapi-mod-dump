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
    /// <summary>
    /// List of all fishdata that use a TrapSpawnConditions
    /// </summary>
    public static List<FishData> TrapData;

    public FishCatalogue()
    {
        LoadFishData();
        LoadLocationData();
    }

    /// <summary>
    /// Returns all fish that the player could catch at the current location
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<FishData> GetCurrentlyAvailableFish(bool include_legendaries = false)
    {
        return GetCurrentLocationFish(include_legendaries).Where(fish_data => fish_data.CanBeCaughtHere());
    }

    public static IEnumerable<FishData> GetCurrentLocationFish(bool include_legendaries = false)
    {
        if (!LocationFishData.ContainsKey(Game1.currentLocation.Name))
            yield break;

        // truth table
        // include_legendaries, fish.legendary | result
        // true, true | true
        // true, false | true
        // false, true | false
        // false, false | true
        IEnumerable<FishData> fishes = LocationFishData[Game1.currentLocation.Name]
            .Where(f => include_legendaries || (!include_legendaries && !f.IsLegendary));

        foreach (FishData fish_data in fishes)
            yield return fish_data;
    }

    public static IEnumerable<FishData> GetCurrentLocationTrappers()
    {
        return TrapData.Where(fish_data => fish_data.CanBeCaughtHere());
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
                // qi's fish aren't boss fish in their respective SpawnFishData, but they should be imo
                bool is_boss = fish_data.IsBossFish || (fish_data.Condition is not null && fish_data.Condition.Contains("LEGENDARY"));
                GenerateFishData(loc, fish_data.ItemId, is_boss);
            }
        }
        AddMissingFish();

        // Add trapper fish
        TrapData = AllFishSpawnConditions.Where(kvp => kvp.Value is TrapSpawnConditions)
            .Select(kvp => {
                FishData fd = new(FishData.WildcardLocation, TrappersQualifiedID(kvp.Key));
                fd.AddSpawnConditions(kvp.Value);
                return fd;
            })
            .ToList();

    }

    /// <summary>
    /// Some fish are missing locations, this functions adds them "manually"
    /// </summary>
    private void AddMissingFish()
    {
        GenerateFishData("UndergroundMine20", "(O)158"); // stone fish
        GenerateFishData("UndergroundMine20", "(O)156"); // Ghost fish (lvl 20)
        GenerateFishData("UndergroundMine60", "(O)161"); // ice pip
        GenerateFishData("UndergroundMine60", "(O)156"); // Ghost fish (lvl 60)
        GenerateFishData("UndergroundMine100", "(O)162"); // lava eel
    }

    /// <summary>
    /// Generates fish data and adds fish to AllFishData and LocationFishData
    /// </summary>
    /// <param name="loc"></param>
    /// <param name="qualified_id"></param>
    private void GenerateFishData(string loc, string qualified_id, bool is_legendary=false)
    {
        if (AllFishData.ContainsKey(qualified_id))
        {
            AllFishData[qualified_id].AddLocation(loc);
        }
        else
        {
            FishData fd = new FishData(loc, qualified_id, is_legendary);
            SpawnConditions spawnConditions = AllFishSpawnConditions.Values
                .FirstOrDefault(spawn_cond => spawn_cond.FishName == fd.FishItem.Name);

            fd.AddSpawnConditions(spawnConditions);
            AllFishData[qualified_id] = fd;
        }

        if (!LocationFishData.ContainsKey(loc))
            LocationFishData[loc] = new();
        LocationFishData[loc].Add(AllFishData[qualified_id]);
    }

    private string TrappersQualifiedID(string trapper_name)
    {
        switch (trapper_name)
        {
            case "Lobster": return "(O)715";
            case "Crayfish": return "(O)716";
            case "Crab": return "(O)717";
            case "Cockle": return "(O)718";
            case "Mussel": return "(O)719";
            case "Shrimp": return "(O)720";
            case "Snail": return "(O)721";
            case "Oyster": return "(O)723";
            case "Clam": return "(O)372";
            case "Periwinkle": return "(O)722";
            default: throw new ArgumentException($"Unknown trapper name: {trapper_name}");
        }
    }
}
