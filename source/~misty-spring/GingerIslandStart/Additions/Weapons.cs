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
using StardewValley.Extensions;
using StardewValley.GameData.Shops;
using StardewValley.GameData.Weapons;
// ReSharper disable PossibleLossOfFraction

namespace GingerIslandStart.Additions;

public static class Weapons
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    private static readonly string[] Levels = { "Copper", "Iron", "Gold", "Iridium" };
    private static string Translation(string key) => ModEntry.Help.Translation.Get(key);
    private static string Id => ModEntry.Id;
    public static Dictionary<string,WeaponData> Create()
    {
        var result = new Dictionary<string, WeaponData>();

        var index = 0;
        foreach (var type in Levels)
        {
            result.Add($"{Id}_{type}Sword", GetData(type, index));
            index++;
        }

        return result;
    }

    private static WeaponData GetData(string type, int index)
    {
        Log($"Creating weapon data for type {type}...");
        
        var level = index + 1;
        
        return new WeaponData()
        {
            Name = $"{Id}_{type}Sword",
            DisplayName = $"[LocalizedText Strings/Weapons:{Id}_{type}Sword]",
            Description = "[LocalizedText Strings/Weapons:Cutlass_Description]",
            MinDamage = 15 * level,
            MaxDamage = 20 * level,
            Knockback = 1 + level / 10,
            //Speed = -2 + level,
            Precision = 0,
            Defense = 0,
            Type = 0,
            AreaOfEffect = 2,
            CritChance = (float)(0.03 + level / 100),
            CritMultiplier = 3,
            CanBeLostOnDeath = false,
            Texture = $"Mods/{Id}/weapons",
            SpriteIndex = index,
        };
    }

    internal static Dictionary<string, string> GetNames()
    {
        var result = new Dictionary<string, string>();

        foreach (var type in Levels)
        {
            var ore = Game1.content.LoadString("Strings/StringsFromCSFiles:" + GetDebris(type));
            var swordName = string.Format(Translation("Sword_type"), ore);
            
            result.Add($"{Id}_{type}Sword", swordName);
        }

        return result;
    }
    
    private static string GetDebris(string type)
    {
        var number = type switch
        {
            "Iron" => 622,
            "Gold" => 624,
            "Iridium" => 626,
            _ => 621
        };

        return $"Debris.cs.{number}";
    }

    public static IEnumerable<ShopItemData> GetShopData()
    {
        var result = new List<ShopItemData>();

        var index = 0;
        foreach (var type in Levels)
        {
            result.Add(new ShopItemData
            {
                Id = $"{Id}_{type}Sword",
                ItemId = $"{Id}_{type}Sword",
                MaxStack = 1,
                IsRecipe = false,
                Condition = null,
                TradeItemId = $"(O){334 + index}",
                TradeItemAmount = 5,
                Price = GetPrice(index),
                AvailableStock = 1,
                AvailableStockLimit = LimitedStockMode.Player,
                AvoidRepeat = true
            });
            index++;
        }

        return result;
    }

    private static int GetPrice(int index)
    {
        var basePrice= index switch
        {
            1 => 5000,
            2 => 10000,
            3 => 25000,
            _ => 2000
        };
        
        return basePrice + 500;
    }

    /// <summary>
    /// Gets a random weapon depending on luck.
    /// </summary>
    /// <returns></returns>
    internal static ShopItemData Random()
    {
        var r = Game1.random;
        var luck = Game1.player.DailyLuck;
        var price = (int)(5000 * ModEntry.GeneralDifficultyMultiplier);

        var id = 0;
        
        var lowWeapons = new[] { 24, 43, 1, 49, 3 };
        var midWeapons = new[] { 14, 5, 7};
        var highWeapons = new[] { 52, 60 };
        var dwarfWeapons = new[] { 56, 55, 54 };

        id = luck switch
        {
            > 0.07 => Game1.random.ChooseFrom(dwarfWeapons),
            > 0.02 and <= +0.07 => r.ChooseFrom(highWeapons),
            0 or >= -0.02 and <= +0.02 => r.ChooseFrom(midWeapons),
            < -0.02 and >= -0.07 => r.ChooseFrom(lowWeapons),
            < -0.07 => 22,
            _ => id
        };

        if (dwarfWeapons.Contains(id))
            price += (int)(1500 * ModEntry.GeneralDifficultyMultiplier);

        return new ShopItemData { AvailableStock = 1, Id = "Sword", ItemId = $"(W){id}", Price = price};
    }
}