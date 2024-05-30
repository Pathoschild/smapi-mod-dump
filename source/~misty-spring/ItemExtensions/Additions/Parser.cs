/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using ItemExtensions.Models;
using ItemExtensions.Models.Contained;
using ItemExtensions.Models.Enums;
using ItemExtensions.Models.Items;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace ItemExtensions.Additions;

public static class Parser
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);

    internal static void ObjectData(Dictionary<string, ItemData> objData)
    {
        ModEntry.Data = new Dictionary<string, ItemData>();
        foreach(var obj in objData)
        {
            Log($"Checking {obj.Key} data...");

            var light = obj.Value.Light;
            if (light != null)
            {
                if (light.Size == 0)
                {
                    Log($"Item light can't be 0. Skipping {obj.Key}.", LogLevel.Warn);
                    continue;
                }
                
                if(light.Transparency == 0)
                {
                    Log($"Item transparency can't be 0. Skipping {obj.Key}.", LogLevel.Warn);
                    continue;
                }
            }
            
            //add to items
            ModEntry.Data.Add(obj.Key, obj.Value);
            
            #if DEBUG
            Log("Added successfully.");
            #endif
        }
    }

    public static void EatingAnimations(Dictionary<string, FarmerAnimation> animations)
    {
        ModEntry.EatingAnimations = new Dictionary<string, FarmerAnimation>();
        foreach(var anim in animations)
        {
            if(anim.Key.StartsWith("base") == false)
                Log($"Checking {anim.Key} data...");
            
            if(!anim.Value.IsValid(out var parsed))
                continue;
            
            //add to items
            ModEntry.EatingAnimations.Add(anim.Key, parsed);
        }
    }

    public static void Resources(Dictionary<string, ResourceData> clumps, bool skipTextureCheck = false)
    {
        Log($"Beginning to parse data. (Raw entries: {clumps?.Count ?? 0})");

        if (clumps is null)
            return;
        
        ModEntry.Ores = new Dictionary<string, ResourceData>();
        ModEntry.BigClumps = new Dictionary<string, ResourceData>();
        foreach(var (id, data) in clumps)
        {
            if(string.IsNullOrWhiteSpace(id))
                continue;
            
            Log($"Checking {id} data...");

            if(data.IsValid(skipTextureCheck) == false)
                continue;

            //check it's not vanilla
            if (int.TryParse(id, out var asInt))
            {
                //if it's a vanilla ID and not ore, ignore
                if (asInt < 1000 && GeneralResource.VanillaIds.Contains(asInt) == false)
                    continue;
                
                data.Trim(asInt);
            }
            
            //add depending on size
            if(data.Width > 1 || data.Height > 1)
                ModEntry.BigClumps.Add(id, data);
            else
                ModEntry.Ores.Add(id, data);
        }
        
        Log($"Loaded {ModEntry.Ores?.Count ?? 0} custom nodes, and {ModEntry.BigClumps?.Count ?? 0} resource clumps.");

        Log("Invalidating asset 'Data/Objects'...");
        ModEntry.Help.GameContent.InvalidateCache("Data/Objects");
    }

    public static void MixedSeeds(Dictionary<string, List<MixedSeedData>> seeds)
    {
        ModEntry.Seeds = new Dictionary<string, List<MixedSeedData>>();
        foreach(var pair in seeds)
        {
            Log($"Checking {pair.Key} data...");

            var validSeeds = new List<MixedSeedData>();
            var hasAllSeeds = true;
            
            foreach (var data in pair.Value)
            {
                //checks id
                if (data.IsValid())
                {
                    //checks sub conditions like having a mod and season
                    if (data.CheckConditions())
                    {
                        validSeeds.Add(data);
                    }
                    
                    continue;
                }
                
                hasAllSeeds = false;
                break;
            }
    
            //add depending on size
            if(hasAllSeeds == false)
                continue;
            else
                ModEntry.Seeds.Add(pair.Key, validSeeds);
        }
    }

    public static void Panning(Dictionary<string, PanningData> panData)
    {
        ModEntry.Panning = new List<PanningData>();
        foreach(var pair in panData)
        {
            Log($"Checking {pair.Key} data...");

            //checks id
            if (string.IsNullOrWhiteSpace(pair.Value.ItemId) && (pair.Value.RandomItemId is null || pair.Value.RandomItemId.Any() == false))
            {
                Log($"Panning item with key '{pair.Key}' doesn't have an item ID. Skipping", LogLevel.Info);
                continue;
            }
            
            ModEntry.Panning.Add(pair.Value);
        }
    }

    internal static void Terrain(Dictionary<string, TerrainSpawnData> trees)
    {
        ModEntry.MineTerrain = new Dictionary<string,TerrainSpawnData>();
        foreach (var pair in trees)
        {
            Log($"Checking {pair.Key} data...");

            //checks id
            if (string.IsNullOrWhiteSpace(pair.Value.TerrainFeatureId))
            {
                Log($"Mineshaft spawn with key '{pair.Key}' doesn't have an ID. Skipping", LogLevel.Info);
                continue;
            }
            if (pair.Value.Type == FeatureType.FruitTree && Game1.fruitTreeData.ContainsKey(pair.Value.TerrainFeatureId) == false)
            {
                Log($"Mineshaft spawn with key '{pair.Key}' has a fruit tree id that doesn't exist. Skipping", LogLevel.Info);
                continue;
            }

            ModEntry.MineTerrain.Add(pair.Key, pair.Value);
        }
    }

    internal static void Train(Dictionary<string, TrainDropData> trainData)
    {
        ModEntry.TrainDrops = new Dictionary<string, TrainDropData>();
        foreach (var pair in trainData)
        {
            Log($"Checking {pair.Key} data...");

            //checks id
            if (string.IsNullOrWhiteSpace(pair.Value.ItemId))
            {
                Log($"Train drop with key '{pair.Key}' has empty item ID. Skipping", LogLevel.Info);
                continue;
            }

            ModEntry.TrainDrops.Add(pair.Key, pair.Value);
        }
    }

    /// <summary>
    /// Checks an Id.
    /// </summary>
    /// <param name="id">Qualified item ID</param>
    /// <returns>Whether the Id is a vanilla node.</returns>
    internal static bool IsVanilla(string id)
    {
        if (int.TryParse(id, out var asInt))
            return asInt < 931;

        return id switch {
                "VolcanoGoldNode" => true,
                "VolcanoCoalNode0" => true,
                "VolcanoCoalNode1" => true,
                "BasicCoalNode0" => true,
                "BasicCoalNode1" => true,
                "GreenRainWeeds7" => true,
                "GreenRainWeeds6" => true,
                "GreenRainWeeds5" => true,
                "GreenRainWeeds4" => true,
                "GreenRainWeeds3" => true,
                "GreenRainWeeds2" => true,
                "GreenRainWeeds1" => true,
                "GreenRainWeeds0" => true,
                "CalicoEggStone_0" => true,
                "CalicoEggStone_1" => true,
                "CalicoEggStone_2" => true,
                "FarAwayStone" => true,
                _ => false
        };
    }

    internal static List<string> SplitCommas(string str)
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(str))
            return result;

        var str2 = str.Replace(", ", ",");
        if (string.IsNullOrWhiteSpace(str))
            return result;

        foreach (var separated in str2.Split(','))
        {
            result.Add(separated);
        }
        return result;
    }
}
