/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace ItemExtensions.Models;

public class TreeSpawnData
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    private static IModHelper Helper => ModEntry.Help;
    
    //public string TreeId { get; set; }
    public int GrowthStage { get; set; }
  
    //conditional
    public List<MineSpawn> MineSpawns { get; set; } = new();
    //conditional
    internal List<MineSpawn> RealSpawnData { get; set; } = new();

    public bool IsValid()
    {
      /*
        if (string.IsNullOrWhiteSpace(TreeId))
        {
            Log("Must specify tree ID! Skipping", LogLevel.Warn);
            return false;
        }*/

         foreach (var floorData in MineSpawns)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(floorData.Floors))
                    continue;
                
                floorData.Parse(GetFloors(floorData.Floors)); 
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
