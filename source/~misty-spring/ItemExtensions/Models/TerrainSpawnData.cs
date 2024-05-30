/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using ItemExtensions.Models.Contained;
using ItemExtensions.Models.Enums;
using StardewModdingAPI;
using StardewValley;

namespace ItemExtensions.Models;

public class TerrainSpawnData
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    private static IModHelper Helper => ModEntry.Help;
    
    //which feature to create
    public string TerrainFeatureId { get; set; }
    public FeatureType Type { get; set; } = FeatureType.Tree;
    public int Health { get; set; } = -1;
    
    //configurable for trees
    public int GrowthStage { get; set; } = -1; //0 seed, 1 sprout, 2 sapling, 3 bush, 5 tree
    //only for wild trees
    public double MossChance { get; set; }
    public bool Stump { get; set; }
    //only for fruit trees
    public int FruitAmount { get; set; } = 3; //0 seed, 1 sprout, 2 sapling, 3 bush, 5 tree
  
    //conditional
    public int Amount { get; set; } = 1; 
    public List<MineSpawn> MineSpawns { get; set; } = new();
    internal List<MineSpawn> RealSpawnData { get; set; } = new();

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(TerrainFeatureId))
        {
            Log("Must specify terrain feature ID! Skipping", LogLevel.Warn);
            return false;
        }
        if (Type == FeatureType.None)
        {
            Log("Must specify terrain feature type! Skipping", LogLevel.Warn);
            return false;
        }

         foreach (var floorData in MineSpawns)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(floorData.Floors))
                    continue;
                
                floorData.Parse(ResourceData.GetFloors(floorData.Floors)); 
                RealSpawnData.Add(floorData);
            }
            catch (Exception e)
            {
                //silent error because it might not happen unless there's no data, i think.
                Log($"Error: {e}");
                return false;
            }
        }

        return true;
    }
}
