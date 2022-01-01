/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/FarmCaveSpawn
**
*************************************************/

namespace FarmCaveSpawn;
class ModConfig
{
    public int MaxDailySpawns { get; set; } = 6; //Maximum number of spawns.

    public float SpawnChance { get; set; } = 5f; //probability of any tile spawning a thing.
    public float TreeFruitChance { get; set; } = 50f; //probability of the spawn being a tree fruit.
    public bool IgnoreFarmCaveType { get; set; } = false; //should I spawn fruits regardless of the farm cave type?
    public bool EarlyFarmCave { get; set; } = false; //allow spawn of fruits even before Demetrius shows up.
    public bool UseModCaves { get; set; } = true;//use caves found in the additional locations list!
    public bool UseMineCave { get; set; } = false; //allow spawn of fruits into the mine cave (after fruit cave)
    public bool SeasonalOnly { get; set; } = false; //limit to just seasonal tree fruit.
    public bool AllowAnyTreeProduct { get; set; } = true;
    public bool EdiblesOnly { get; set; } = true;
    public bool NoBananasBeforeShrine { get; set; } = true;
    public int PriceCap { get; set; } = 200;
}