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

#pragma warning disable SA1623 // Property summary documentation should match accessors
#pragma warning disable SA1201 // Elements should appear in the correct order - fields are kept next to their accessors for this class.
/// <summary>
/// Configuration class for this mod.
/// </summary>
public class ModConfig
{
    private int maxDailySpawns = 6;

    /// <summary>
    /// Maximum number of spawns per day.
    /// </summary>
    public int MaxDailySpawns
    {
        get => this.maxDailySpawns;
        set => this.maxDailySpawns = Math.Clamp(value, 0, 100);
    }

    private float spawnChance = 3f;

    /// <summary>
    /// Probability of any tile spawning an object, capped by max daily spawns.
    /// </summary>
    public float SpawnChance
    {
        get => this.spawnChance;
        set => this.spawnChance = Math.Clamp(value, 0f, 100f);
    }

    private float treeFruitChance = 20f;

    /// <summary>
    /// Probability of any particular spawn being a tree fruit item.
    /// </summary>
    public float TreeFruitChance
    {
        get => this.treeFruitChance;
        set => this.treeFruitChance = Math.Clamp(value, 0f, 100f);
    }

    /// <summary>
    /// Should spawn in fruit after the Demetrius cutscene is seen, regardless of choice.
    /// </summary>
    /// <remarks>Checks for caveChoice, but also FarmCaveFarmework.</remarks>
    public bool IgnoreFarmCaveType { get; set; } = false;

    /// <summary>
    /// Should I allow fruit spawning even before Demeterius shows up.
    /// </summary>
    /// <remarks>Checks for caveChoice, but also FarmCaveFramework.</remarks>
    public bool EarlyFarmCave { get; set; } = false;

    /// <summary>
    /// Should I check the additional locations list.
    /// </summary>
    public bool UseModCaves { get; set; } = true;

    /// <summary>
    /// Should I use the mine cave entrance as well.
    /// </summary>
    public bool UseMineCave { get; set; } = false;

    /// <summary>
    /// Use only the six vanilla tree fruit + the four vanilla forage fruit.
    /// </summary>
    public bool UseVanillaFruitOnly { get; set; } = false;

    /// <summary>
    /// Should I limit myself to just fruits in season?.
    /// </summary>
    public bool SeasonalOnly { get; set; } = false;

    /// <summary>
    /// Should I allow any fruit tree product, even if it's not categorized as fruit.
    /// </summary>
    public bool AllowAnyTreeProduct { get; set; } = true;

    /// <summary>
    /// Restrict to only edible items.
    /// </summary>
    /// <remarks>Sometimes inexplicable things in the game have positive edibility....</remarks>
    public bool EdiblesOnly { get; set; } = true;

    /// <summary>
    /// Remove bananas from the pool before a specific vanilla quest is done.
    /// </summary>
    public bool NoBananasBeforeShrine { get; set; } = true;

    /// <summary>
    /// Caps the price of fruit you can get.
    /// </summary>
    public int PriceCap { get; set; } = 200;
}
#pragma warning restore SA1623 // Property summary documentation should match accessors
#pragma warning restore SA1201 // Elements should appear in the correct order
