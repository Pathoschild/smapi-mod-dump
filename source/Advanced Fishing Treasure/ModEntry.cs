/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aceynk/AdvancedFishingTreasure
**
*************************************************/


using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;
using GenericModConfigMenu;
using HarmonyLib;
using Sickhead.Engine.Util;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Constants;
using StardewValley.Extensions;
using StardewValley.GameData.Objects;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewValley.WorldMaps;
using Item = StardewValley.Item;
using Object = StardewValley.Object;

namespace AdvancedFishingTreasure;

public class ModEntry : Mod
{
    public static void Log(string v)
    {
        _log.Log(v, LogLevel.Debug);
    }

    public static IMonitor _log = null!;
    public static Dictionary<string, List<IdItemPair>> CachedItems = new();
    public static ModConfig Config;
    public static List<string> Blacklisted = new()
    {
	    "(T)CopperTrashCan", "(T)SteelTrashCan", "(T)GoldTrashCan", "(T)IridiumTrashCan",
	    "(T)Lantern", "(BC)22", "(BC)23", "(O)30", "(O)590", "(O)858", "(O)925", "(O)927",
	    "(O)929", "(O)930", "(O)GoldCoin", "(O)SeedSpot", "(O)73"
    };
    
    public override void Entry(IModHelper helper)
    {
        Config = Helper.ReadConfig<ModConfig>();
        _log = Monitor;
        
        Helper.Events.GameLoop.SaveLoaded += GameStarted;
        Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        
        //var log = Monitor.Log;
        var harmony = new Harmony("Aceynk.AdvancedFishingTreasure");
        
        // patch StardewValley.Tools.FishingRod.openTreasureMenuEndFunction
        // postfix edit 

        try
        {
            var treasureFunc =
                AccessTools.Method(typeof(FishingRod), nameof(FishingRod.openTreasureMenuEndFunction));
            var postFunc = SymbolExtensions.GetMethodInfo((int remainingFish, FishingRod __instance) => Patches.TreasureMenuPatch.Postfix(remainingFish, __instance));
            var preFunc = SymbolExtensions.GetMethodInfo((int remainingFish, FishingRod __instance) =>
                Patches.TreasureMenuPatch.Prefix(remainingFish, __instance));
            
            //var grabMenuFunc = AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu));
            //var preFunc = SymbolExtensions.GetMethodInfo((IList<Item> inventory, object context) => Patches.GrabMenuPatch.Prefix(inventory, context));

            harmony.Patch(treasureFunc, prefix: new HarmonyMethod(preFunc), postfix: new HarmonyMethod(postFunc));
            //harmony.Patch(grabMenuFunc, prefix: new HarmonyMethod(preFunc));
        }
        catch (Exception e)
        {
            Log($"An error occurred while patching. Error log: {e}");
        }
        
        Log("Successfully patched openTreasureMenuEndFunction");
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs svArgs)
    {
        var menu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (menu is null)
        {
            return;
        }
        
        menu.Register(
            mod: ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => Helper.WriteConfig(Config)
        );
        
        menu.AddSectionTitle(
            mod: ModManifest,
            text: () => "Main"
        );
        
        // MAIN
        
        menu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("GMCM.ModEnabled.Name"),
            tooltip: () => Helper.Translation.Get("GMCM.ModEnabled.Tooltip"),
            getValue: () => Config.ModEnabled,
            setValue: value => Config.ModEnabled = value
        );
        
        menu.AddNumberOption(
			mod: ModManifest,
			name: () => Helper.Translation.Get("GMCM.VanillaMultiplier.Name"),
			tooltip: () => Helper.Translation.Get("GMCM.VanillaMultiplier.Tooltip"),
			getValue: () => Config.VanillaMultiplier,
			setValue: v => Config.VanillaMultiplier = v,
			min: 0,
			max: 50
        );
        
        menu.AddNumberOption(
			mod: ModManifest,
			name: () => Helper.Translation.Get("GMCM.ModdedMultiplier.Name"),
			tooltip: () => Helper.Translation.Get("GMCM.ModdedMultiplier.Tooltip"),
			getValue: () => Config.ModdedMultiplier,
			setValue: v => Config.ModdedMultiplier = v,
			min: 0,
			max: 50
        );
        
        menu.AddBoolOption(
	        mod: ModManifest,
	        name: () => Helper.Translation.Get("GMCM.EnableBlacklist.Name"),
	        tooltip: () => Helper.Translation.Get("GMCM.EnableBlacklist.Tooltip"),
	        getValue: () => Config.EnableBlacklist,
	        setValue: value => Config.EnableBlacklist = value
        );
        
        menu.AddNumberOption(
	        mod: ModManifest,
	        name: () => Helper.Translation.Get("GMCM.MoneyPrize.Name"),
	        tooltip: () => Helper.Translation.Get("GMCM.MoneyPrize.Tooltip"),
	        getValue: () => Config.MoneyPrize,
	        setValue: v => Config.MoneyPrize = v
        );
        
        menu.AddBoolOption(
	        mod: ModManifest,
	        name: () => Helper.Translation.Get("GMCM.VanillaFluctuation.Name"),
	        tooltip: () => Helper.Translation.Get("GMCM.VanillaFluctuation.Tooltip"),
	        getValue: () => Config.VanillaFluctuation,
	        setValue: value => Config.VanillaFluctuation = value
        );
        
        menu.AddNumberOption(
	        mod: ModManifest,
	        name: () => Helper.Translation.Get("GMCM.PriceMin.Name"),
	        tooltip: () => Helper.Translation.Get("GMCM.PriceMin.Tooltip"),
	        getValue: () => Config.PriceMin,
	        setValue: v => Config.PriceMin = v
        );
        
        menu.AddNumberOption(
	        mod: ModManifest,
	        name: () => Helper.Translation.Get("GMCM.PriceMax.Name"),
	        tooltip: () => Helper.Translation.Get("GMCM.PriceMax.Tooltip"),
	        getValue: () => Config.PriceMax,
	        setValue: v => Config.PriceMax = v
        );
        
        menu.AddNumberOption(
	        mod: ModManifest,
	        name: () => Helper.Translation.Get($"GMCM.ChestChance.Name"),
	        tooltip: () => Helper.Translation.Get($"GMCM.ChestChance.Tooltip"),
	        getValue: () => Config.ChestChance,
	        setValue: value => Config.ChestChance = value,
	        min: 0,
	        max: 100
        );
        
        // QUALITY
        
        menu.AddSectionTitle(
	        mod: ModManifest,
	        text: () => "Quality",
	        tooltip: () => "Evaluated Iridium to Normal."
        );
        
        menu.AddBoolOption(
	        mod: ModManifest,
	        name: () => Helper.Translation.Get("GMCM.AllowSilver.Name"),
	        tooltip: () => Helper.Translation.Get("GMCM.AllowSilver.Tooltip"),
	        getValue: () => Config.AllowSilver,
	        setValue: value => Config.AllowSilver = value
        );
        
        menu.AddNumberOption(
	        mod: ModManifest,
	        name: () => Helper.Translation.Get("GMCM.SilverProb.Name"),
	        tooltip: () => Helper.Translation.Get("GMCM.SilverProb.Tooltip"),
	        getValue: () => Config.SilverProb,
	        setValue: v => Config.SilverProb = v,
	        min: 0,
	        max: 100
        );
        
        menu.AddBoolOption(
	        mod: ModManifest,
	        name: () => Helper.Translation.Get("GMCM.AllowGold.Name"),
	        tooltip: () => Helper.Translation.Get("GMCM.AllowGold.Tooltip"),
	        getValue: () => Config.AllowGold,
	        setValue: value => Config.AllowGold = value
        );
        
        menu.AddNumberOption(
	        mod: ModManifest,
	        name: () => Helper.Translation.Get("GMCM.GoldProb.Name"),
	        tooltip: () => Helper.Translation.Get("GMCM.GoldProb.Tooltip"),
	        getValue: () => Config.GoldProb,
	        setValue: v => Config.GoldProb = v,
	        min: 0,
	        max: 100
        );
        
        menu.AddBoolOption(
	        mod: ModManifest,
	        name: () => Helper.Translation.Get("GMCM.AllowIridium.Name"),
	        tooltip: () => Helper.Translation.Get("GMCM.AllowIridium.Tooltip"),
	        getValue: () => Config.AllowIridium,
	        setValue: value => Config.AllowIridium = value
        );
        
        menu.AddNumberOption(
	        mod: ModManifest,
	        name: () => Helper.Translation.Get("GMCM.IridiumProb.Name"),
	        tooltip: () => Helper.Translation.Get("GMCM.IridiumProb.Tooltip"),
	        getValue: () => Config.IridiumProb,
	        setValue: v => Config.IridiumProb = v,
	        min: 0,
	        max: 100
        );
        
        // GOLDEN TREASURE CHESTS
        
        menu.AddSectionTitle(
	        mod: ModManifest,
	        text: () => "Golden Treasure Chests",
	        tooltip: () => "Config options to customize the bonuses of golden treasure chests."
        );
        
        menu.AddNumberOption(
	        mod: ModManifest,
	        name: () => Helper.Translation.Get("GMCM.GoldPrizeMult.Name"),
	        tooltip: () => Helper.Translation.Get("GMCM.GoldPrizeMult.Tooltip"),
	        getValue: () => Config.GoldPrizeMult,
	        setValue: v => Config.GoldPrizeMult = v,
	        min: 0,
	        max: 10
        );
        
        menu.AddNumberOption(
	        mod: ModManifest,
	        name: () => Helper.Translation.Get("GMCM.GoldPriceMaxMult.Name"),
	        tooltip: () => Helper.Translation.Get("GMCM.GoldPriceMaxMult.Tooltip"),
	        getValue: () => Config.GoldPriceMaxMult,
	        setValue: v => Config.GoldPriceMaxMult = v,
	        min: -1,
	        max: 20
        );
        
        menu.AddNumberOption(
	        mod: ModManifest,
	        name: () => Helper.Translation.Get("GMCM.GoldBonusRolls.Name"),
	        tooltip: () => Helper.Translation.Get("GMCM.GoldBonusRolls.Tooltip"),
	        getValue: () => Config.GoldBonusRolls,
	        setValue: v => Config.GoldBonusRolls = v,
	        min: 0,
	        max: 10
        );
        
        // INCLUDED ITEMS

        foreach (var property in typeof(ModConfig).GetProperties())
        {
	        if (property.PropertyType == typeof(bool) && property.Name.StartsWith("Include"))
	        {
		        menu.AddBoolOption(
			        mod: ModManifest,
			        name: () => Helper.Translation.Get($"GMCM.{property.Name}.Name"),
			        tooltip: () => Helper.Translation.Get($"GMCM.{property.Name}.Tooltip"),
			        getValue: () => (bool)property.GetValue(Config)!,
			        setValue: value => property.SetValue(Config, value)
		        );
	        }
	        else if (property.PropertyType == typeof(int) && property.Name.EndsWith("Chance"))
	        {
		        if (property.Name != "ChestChance")
			        menu.AddNumberOption(
				        mod: ModManifest,
				        name: () => Helper.Translation.Get($"GMCM.{property.Name}.Name"),
				        tooltip: () => Helper.Translation.Get($"GMCM.{property.Name}.Tooltip"),
				        getValue: () => (int)property.GetValue(Config)!,
				        setValue: value => property.SetValue(Config, value),
				        min: 0,
				        max: 100
			        );

		        if (property.Name == "ChestChance")
		        {
			        menu.AddSectionTitle(
				        mod: ModManifest,
				        text: () => "Included Items"
			        );
			        
			        menu.AddTextOption(
				        mod: ModManifest,
				        name: () => Helper.Translation.Get("GMCM.AllowedContext.Name"),
				        tooltip: () => Helper.Translation.Get("GMCM.AllowedContext.Tooltip"),
				        getValue: () => Config.AllowedContext,
				        setValue: v => Config.AllowedContext = v
			        );
		        
			        menu.AddNumberOption(
				        mod: ModManifest,
				        name: () => Helper.Translation.Get("GMCM.ContextProb.Name"),
				        tooltip: () => Helper.Translation.Get("GMCM.ContextProb.Tooltip"),
				        getValue: () => Config.ContextProb,
				        setValue: v => Config.ContextProb = v,
				        min: 0,
				        max: 100
			        );
		        }
	        }
        }
        
        menu.AddSectionTitle(
			mod: ModManifest,
			text: () => "Excluded Items"
        );
        
        // EXCLUDED ITEMS
        
        menu.AddTextOption(
			mod: ModManifest,
			name: () => Helper.Translation.Get("GMCM.ExcludeVanilla.Name"),
			tooltip: () => Helper.Translation.Get("GMCM.ExcludeVanilla.Tooltip"),
			getValue: () => Config.ExcludeVanilla,
			setValue: v => Config.ExcludeVanilla = v
        );
        
        menu.AddTextOption(
	        mod: ModManifest,
	        name: () => Helper.Translation.Get("GMCM.ExcludeModded.Name"),
	        tooltip: () => Helper.Translation.Get("GMCM.ExcludeModded.Tooltip"),
	        getValue: () => Config.ExcludeModded,
	        setValue: v => Config.ExcludeModded = v
        );
    }

    private void GameStarted(object? sender, SaveLoadedEventArgs svArgs)
    {
        InitItemCache();
        FishingRod.baseChanceForTreasure = Config.ChestChance / 100.0;
    }

    public class IdItemPair
    {
	    public string id = null!;
	    public ObjectData obj = null!;

	    public IdItemPair(string pid, ObjectData pobj)
	    {
		    id = pid;
		    obj = pobj;
	    }
    }

    private static void InitItemCache()
    {
        IDictionary<string, ObjectData> itemData = Game1.objectData;
        
        foreach (string key in itemData.Keys)
        {
	        ObjectData? obj = itemData[key];
            string thisCategory = obj.Category.ToString();

            IdItemPair? cacheObj = new IdItemPair(key, obj);

            // null guard
            if (cacheObj == null || cacheObj.obj == null) continue;
            
            if (!CachedItems.ContainsKey(thisCategory))
            {
                CachedItems.Add(thisCategory, new List<IdItemPair>());
            }

            if (!CachedItems.ContainsKey("-96") && obj.Type == "Ring")
            {
	            CachedItems.Add("-96", new List<IdItemPair>());
            }

            if (obj.Type == "Ring" || key == "801")
            {
	            cacheObj.obj.Category = Object.ringCategory;
	            CachedItems["-96"].Add(cacheObj);
	            continue;
            }
            
            if (!CachedItems.ContainsKey("Arch") && obj.Type == "Arch")
            {
	            CachedItems.Add("Arch", new List<IdItemPair>());
            }

            if (obj.Type == "Arch")
            {
	            CachedItems["Arch"].Add(cacheObj);
	            continue;
            }
            
            CachedItems[thisCategory].Add(cacheObj);
        }

        /*
        foreach (string key in CachedItems.Keys)
        {
            Log($"Cached {CachedItems[key].Count} of Category {key}");
            Log(string.Join(", ", CachedItems[key].Select(k => k.Name)));
        }
        */
    }

    private static List<IdItemPair> GetItemsByCategory(string category)
    {
        /*
         * For reference: (Category # : Explanation)
         * 
         * -999 : Small, breakable obstructions (Stone, Weeds)
         * -103 : Exp Books (Book of Stars, Stardew Valley Almanac)
         * -102 : Skill Books (The Alleyway Buffet, The Art O' Crabbing)
         * -96 : Rings [CP added ones, when in Data/Objects] (Mermaid's Bracelet)
         * -81 : Greens (Wild Horseradish, Daffodil)
         * -80 : Flowers (Sweet Pea, Crocus)
         * -79 : Fruit (Coconut, Cactus Fruit)
         * -75 : Vegetables (Parsnip, Green Bean)
         * -74 : Seeds (Tea Sapling, Rice Shoot)
         * -28 : Monster Loot (Bug Meat, Slime)
         * -27 : Tree Goods (Maple Syrup, Oak Resin)
         * -26 : Artisan Goods (Pale Ale, Mayonnaise)
         * -24 : Floors (Brick Floor, Wood Floor)
         * -23 : Fishing Items (Nautilus Shell, Coral)
         * -22 : Tackle (Spinner, Dressed Spinner)
         * -21 : Bait (Bait, Magnet)
         * -20 : Trash (Joja Cola, Trash)
         * -19 : Farm Boosts (Basic Fertilizer, Quality Fertilizer)
         * -18 : Special Animal Goods (Wool, Duck Feather)
         * -17 : Special Farming Items (Sweet Gem Berry, Truffle)
         * -16 : Components (Clay, Wood)
         * -15 : Ores & Bars (Copper Bar, Iron Bar)
         * -12 : Stones (Alamite, Bixite)
         * -8 : Normal Craftables (Cherry Bomb, Bomb)
         * -7 : Food (Fried Egg, Omelet)
         * -6 : Milk (Milk, Large Milk)
         * -5 : Eggs (Egg, Large Egg)
         * -4 : Fish (Pufferfish, Anchovy)
         * -2 : Gems (Emerald, Aquamarine)
         * 0 : Special Items (Lumber, Trimmed Lucky Purple Shorts)
         */
        
        /*
         * Non-Object Categories:
         * BC : Big Craftables
         * T : Tools
         * W : Weapons
         * F : Furniture
         * B : Boots
         * P : Pants
         * S : Shirts
         * H : Hats
         * TR : Trinkets
         * M : Mannequins
		 */
        
        Log($"GetItemsByCategory called with category {category}");

        return CachedItems[category];
    }

    public static double getTreasureChance(FishingRod rod)
    {
	    if (!Config.VanillaFluctuation) return Config.ChestChance;

	    double chance = Config.ChestChance;
	    Object? bait = rod.GetBait(); // MAKE SURE TO CHECK FOR NULL
	    List<Object?> tackle = rod.GetTackle(); // PROBABLY ALSO CAN BE NULL

	    if (bait is null) {}
	    else if (bait.ItemId == "(O)703")
	    {
		    chance += 15;
	    }

	    foreach (Object tack in tackle)
	    {
		    if (tack is null) continue;
		    if (tack.ItemId == "(O)693")
		    {
			    chance += 5;
		    }
	    }

	    if (Game1.player.professions.Contains(Farmer.pirate))
	    {
		    chance += 15;
	    }
	    
	    chance += Game1.player.LuckLevel * 0.5;

	    return Math.Min(100.0, chance);
    }

    private static List<IdItemPair> GetItemsByCategory(int category)
    {
        return GetItemsByCategory(category.ToString());
    }

    private static readonly Random rnd = new Random();
    
    public static List<Item> ShuffleInventory(List<Item> L)
    {
	    // Source: http://stackoverflow.com/revisions/1262619/1
	    int n = L.Count;
	    while (n > 1) {  
		    n--;  
		    int k = rnd.Next(n + 1);  
		    (L[k], L[n]) = (L[n], L[k]);
	    }

	    return L;
    }

    public static bool ContainsEvery(List<string> original, List<string> items)
    {
	    foreach (var obj in items)
	    {
		    if (obj is null)
		    {
			    continue;
		    }
		    if (!original.Contains(obj))
		    {
			    return false;
		    }
	    }

	    return true;
    }

    public static List<string> GetAllItemsWithContextTags(List<string> tags)
    {
	    List<string> itemOut = new();
	    
	    foreach (string key in CachedItems.Keys)
	    {
		    foreach (IdItemPair data in CachedItems[key])
		    {
			    if (data.obj.ContextTags is null)
			    {
				    continue;
			    }
			    
			    if (ContainsEvery(data.obj.ContextTags, tags))
			    {
				    itemOut.Add(data.id);
			    }
		    }
	    }

	    return itemOut;
    }

    public static List<Item> CondenseDisorganizedInventory(List<Item> inventory)
    {
	    Dictionary<string, Item> composite = new();
	    List<Item> unstackableOut = new();

	    foreach (Item item in inventory)
	    {
		    if (item.maximumStackSize() == 1)
		    {
			    unstackableOut.Add(item);
			    continue;
		    }
		    
		    if (item.ItemId is "340" or "342" or "344" or "348" or "350" or "812" or "447" or "SmokedFish" or "DriedFruit" or "DriedMushrooms" or "SpecificBait")
		    {
			    if (!composite.ContainsKey(item.ItemId + item.DisplayName))
			    {
				    composite.Add(item.ItemId + item.DisplayName, item);
				    continue;
			    }

			    composite[item.ItemId + item.DisplayName].Stack += item.Stack;
			    continue;
		    }

		    if (!composite.ContainsKey(item.ItemId))
		    {
			    composite.Add(item.ItemId, item);
			    continue;
		    }
		    
		    composite[item.ItemId].Stack += item.Stack;
	    }

	    return composite.Values.ToList().Concat(unstackableOut).ToList();
    }
    
    // Extracted from the Decompiled Game:

    public static List<Item> GetVanillaGrabMenu(FishingRod rod, int remainingFish)
    {
	    // TAKEN AND MODIFIED FROM StardewValley.Tools.FishingRod.openTreasureMenuEndFunction
	    
	    var who = rod.lastUser;
	    
	    List<Item> treasures = new();
	    float chance = 1f;
	    
		while (Game1.random.NextDouble() <= chance)
		{
			chance *= (rod.goldenTreasure ? 0.6f : 0.4f);
			if (Game1.IsSpring && !(who.currentLocation is Beach) && Game1.random.NextDouble() < 0.1)
			{
				treasures.Add(ItemRegistry.Create("(O)273", Game1.random.Next(2, 6) + ((Game1.random.NextDouble() < 0.25) ? 5 : 0)));
			}
			if (rod.numberOfFishCaught > 1 && who.craftingRecipes.ContainsKey("Wild Bait") && Game1.random.NextBool())
			{
				treasures.Add(ItemRegistry.Create("(O)774", 2 + ((Game1.random.NextDouble() < 0.25) ? 2 : 0)));
			}
			if (Game1.random.NextDouble() <= 0.33 && who.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
			{
				treasures.Add(ItemRegistry.Create("(O)890", Game1.random.Next(1, 3) + ((Game1.random.NextDouble() < 0.25) ? 2 : 0)));
			}
			while (Utility.tryRollMysteryBox(0.08 + Game1.player.team.AverageDailyLuck() / 5.0))
			{
				treasures.Add(ItemRegistry.Create((Game1.player.stats.Get(StatKeys.Mastery(2)) != 0) ? "(O)GoldenMysteryBox" : "(O)MysteryBox"));
			}
			if (Game1.player.stats.Get(StatKeys.Mastery(0)) != 0 && Game1.random.NextDouble() < 0.05)
			{
				treasures.Add(ItemRegistry.Create("(O)GoldenAnimalCracker"));
			}
			if (rod.goldenTreasure && Game1.random.NextDouble() < 0.5)
			{
				switch (Game1.random.Next(13))
				{
				case 0:
					treasures.Add(ItemRegistry.Create("(O)337", Game1.random.Next(1, 6)));
					break;
				case 1:
					treasures.Add(ItemRegistry.Create("(O)SkillBook_" + Game1.random.Next(5)));
					break;
				case 2:
					treasures.Add(Utility.getRaccoonSeedForCurrentTimeOfYear(Game1.player, Game1.random, 8));
					break;
				case 3:
					treasures.Add(ItemRegistry.Create("(O)213"));
					break;
				case 4:
					treasures.Add(ItemRegistry.Create("(O)872", Game1.random.Next(3, 6)));
					break;
				case 5:
					treasures.Add(ItemRegistry.Create("(O)687"));
					break;
				case 6:
					treasures.Add(ItemRegistry.Create("(O)ChallengeBait", Game1.random.Next(3, 6)));
					break;
				case 7:
					treasures.Add(ItemRegistry.Create("(O)703", Game1.random.Next(3, 6)));
					break;
				case 8:
					treasures.Add(ItemRegistry.Create("(O)StardropTea"));
					break;
				case 9:
					treasures.Add(ItemRegistry.Create("(O)797"));
					break;
				case 10:
					treasures.Add(ItemRegistry.Create("(O)733"));
					break;
				case 11:
					treasures.Add(ItemRegistry.Create("(O)728"));
					break;
				case 12:
					treasures.Add(ItemRegistry.Create("(O)SonarBobber"));
					break;
				}
				continue;
			}
			switch (Game1.random.Next(4))
			{
			case 0:
			{
				if (rod.clearWaterDistance >= 5 && Game1.random.NextDouble() < 0.03)
				{
					treasures.Add(new StardewValley.Object("386", Game1.random.Next(1, 3)));
					break;
				}
				List<int> possibles = new List<int>();
				if (rod.clearWaterDistance >= 4)
				{
					possibles.Add(384);
				}
				if (rod.clearWaterDistance >= 3 && (possibles.Count == 0 || Game1.random.NextDouble() < 0.6))
				{
					possibles.Add(380);
				}
				if (possibles.Count == 0 || Game1.random.NextDouble() < 0.6)
				{
					possibles.Add(378);
				}
				if (possibles.Count == 0 || Game1.random.NextDouble() < 0.6)
				{
					possibles.Add(388);
				}
				if (possibles.Count == 0 || Game1.random.NextDouble() < 0.6)
				{
					possibles.Add(390);
				}
				possibles.Add(382);
				Item treasure = ItemRegistry.Create(Game1.random.ChooseFrom(possibles).ToString(), Game1.random.Next(2, 7) * ((!(Game1.random.NextDouble() < 0.05 + (double)(int)who.luckLevel * 0.015)) ? 1 : 2));
				if (Game1.random.NextDouble() < 0.05 + (double)who.LuckLevel * 0.03)
				{
					treasure.Stack *= 2;
				}
				treasures.Add(treasure);
				break;
			}
			case 1:
				if (rod.clearWaterDistance >= 4 && Game1.random.NextDouble() < 0.1 && who.FishingLevel >= 6)
				{
					treasures.Add(ItemRegistry.Create("(O)687"));
				}
				else if (Game1.random.NextDouble() < 0.25 && who.craftingRecipes.ContainsKey("Wild Bait"))
				{
					treasures.Add(ItemRegistry.Create("(O)774", 5 + ((Game1.random.NextDouble() < 0.25) ? 5 : 0)));
				}
				else if (Game1.random.NextDouble() < 0.11 && who.FishingLevel >= 6)
				{
					treasures.Add(ItemRegistry.Create("(O)SonarBobber"));
				}
				else if (who.FishingLevel >= 6)
				{
					treasures.Add(ItemRegistry.Create("(O)DeluxeBait", 5));
				}
				else
				{
					treasures.Add(ItemRegistry.Create("(O)685", 10));
				}
				break;
			case 2:
				if (Game1.random.NextDouble() < 0.1 && Game1.netWorldState.Value.LostBooksFound < 21 && who != null && who.hasOrWillReceiveMail("lostBookFound"))
				{
					treasures.Add(ItemRegistry.Create("(O)102"));
				}
				else if (who.archaeologyFound.Length > 0)
				{
					if (Game1.random.NextDouble() < 0.25 && who.FishingLevel > 1)
					{
						treasures.Add(ItemRegistry.Create("(O)" + Game1.random.Next(585, 588)));
					}
					else if (Game1.random.NextBool() && who.FishingLevel > 1)
					{
						treasures.Add(ItemRegistry.Create("(O)" + Game1.random.Next(103, 120)));
					}
					else
					{
						treasures.Add(ItemRegistry.Create("(O)535"));
					}
				}
				else
				{
					treasures.Add(ItemRegistry.Create("(O)382", Game1.random.Next(1, 3)));
				}
				break;
			case 3:
				switch (Game1.random.Next(3))
				{
				case 0:
				{
					Item treasure2 = ((rod.clearWaterDistance >= 4) ? ItemRegistry.Create("(O)" + (537 + ((Game1.random.NextDouble() < 0.4) ? Game1.random.Next(-2, 0) : 0)), Game1.random.Next(1, 4)) : ((rod.clearWaterDistance < 3) ? ItemRegistry.Create("(O)535", Game1.random.Next(1, 4)) : ItemRegistry.Create("(O)" + (536 + ((Game1.random.NextDouble() < 0.4) ? (-1) : 0)), Game1.random.Next(1, 4))));
					if (Game1.random.NextDouble() < 0.05 + (double)who.LuckLevel * 0.03)
					{
						treasure2.Stack *= 2;
					}
					treasures.Add(treasure2);
					break;
				}
				case 1:
				{
					if (who.FishingLevel < 2)
					{
						treasures.Add(ItemRegistry.Create("(O)382", Game1.random.Next(1, 4)));
						break;
					}
					Item treasure3;
					if (rod.clearWaterDistance >= 4)
					{
						treasures.Add(treasure3 = ItemRegistry.Create("(O)" + ((Game1.random.NextDouble() < 0.3) ? 82 : Game1.random.Choose(64, 60)), Game1.random.Next(1, 3)));
					}
					else if (rod.clearWaterDistance >= 3)
					{
						treasures.Add(treasure3 = ItemRegistry.Create("(O)" + ((Game1.random.NextDouble() < 0.3) ? 84 : Game1.random.Choose(70, 62)), Game1.random.Next(1, 3)));
					}
					else
					{
						treasures.Add(treasure3 = ItemRegistry.Create("(O)" + ((Game1.random.NextDouble() < 0.3) ? 86 : Game1.random.Choose(66, 68)), Game1.random.Next(1, 3)));
					}
					if (Game1.random.NextDouble() < 0.028 * (double)((float)rod.clearWaterDistance / 5f))
					{
						treasures.Add(treasure3 = ItemRegistry.Create("(O)72"));
					}
					if (Game1.random.NextDouble() < 0.05)
					{
						treasure3.Stack *= 2;
					}
					break;
				}
				case 2:
				{
					if (who.FishingLevel < 2)
					{
						treasures.Add(new StardewValley.Object("770", Game1.random.Next(1, 4)));
						break;
					}
					float luckModifier = (1f + (float)who.DailyLuck) * ((float)rod.clearWaterDistance / 5f);
					if (Game1.random.NextDouble() < 0.05 * (double)luckModifier && !who.specialItems.Contains("14"))
					{
						Item weapon = MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)14"), Game1.random);
						weapon.specialItem = true;
						treasures.Add(weapon);
					}
					if (Game1.random.NextDouble() < 0.05 * (double)luckModifier && !who.specialItems.Contains("51"))
					{
						Item weapon2 = MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)51"), Game1.random);
						weapon2.specialItem = true;
						treasures.Add(weapon2);
					}
					if (Game1.random.NextDouble() < 0.07 * (double)luckModifier)
					{
						switch (Game1.random.Next(3))
						{
						case 0:
							treasures.Add(new Ring((516 + ((Game1.random.NextDouble() < (double)((float)who.LuckLevel / 11f)) ? 1 : 0)).ToString()));
							break;
						case 1:
							treasures.Add(new Ring((518 + ((Game1.random.NextDouble() < (double)((float)who.LuckLevel / 11f)) ? 1 : 0)).ToString()));
							break;
						case 2:
							treasures.Add(new Ring(Game1.random.Next(529, 535).ToString()));
							break;
						}
					}
					if (Game1.random.NextDouble() < 0.02 * (double)luckModifier)
					{
						treasures.Add(ItemRegistry.Create("(O)166"));
					}
					if (who.FishingLevel > 5 && Game1.random.NextDouble() < 0.001 * (double)luckModifier)
					{
						treasures.Add(ItemRegistry.Create("(O)74"));
					}
					if (Game1.random.NextDouble() < 0.01 * (double)luckModifier)
					{
						treasures.Add(ItemRegistry.Create("(O)127"));
					}
					if (Game1.random.NextDouble() < 0.01 * (double)luckModifier)
					{
						treasures.Add(ItemRegistry.Create("(O)126"));
					}
					if (Game1.random.NextDouble() < 0.01 * (double)luckModifier)
					{
						treasures.Add(new Ring("527"));
					}
					if (Game1.random.NextDouble() < 0.01 * (double)luckModifier)
					{
						treasures.Add(ItemRegistry.Create("(B)" + Game1.random.Next(504, 514)));
					}
					if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") && Game1.random.NextDouble() < 0.01 * (double)luckModifier)
					{
						treasures.Add(ItemRegistry.Create("(O)928"));
					}
					if (treasures.Count == 1)
					{
						treasures.Add(ItemRegistry.Create("(O)72"));
					}
					if (Game1.player.stats.Get("FishingTreasures") > 3)
					{
						Random r = Utility.CreateRandom(Game1.player.stats.Get("FishingTreasures") * 27973, Game1.uniqueIDForThisGame);
						if (r.NextDouble() < 0.05 * (double)luckModifier)
						{
							treasures.Add(ItemRegistry.Create("(O)SkillBook_" + r.Next(5)));
							chance = 0f;
						}
					}
					break;
				}
				}
				break;
			}
		}
		if (treasures.Count == 0)
		{
			treasures.Add(ItemRegistry.Create("(O)685", Game1.random.Next(1, 4) * 5));
		}
		if (who.hasQuest("98765") && Utility.GetDayOfPassiveFestival("DesertFestival") == 3 && !who.Items.ContainsId("GoldenBobber", 1))
		{
			treasures.Clear();
			treasures.Add(ItemRegistry.Create("(O)GoldenBobber"));
		}
		if (Game1.random.NextDouble() < 0.25 && who.stats.Get("Book_Roe") != 0)
		{
			Item fish = mCreateFish(rod);
			if (fish is StardewValley.Object)
			{
				ColoredObject roe = ItemRegistry.GetObjectTypeDefinition().CreateFlavoredRoe(fish as StardewValley.Object);
				roe.Stack = Game1.random.Next(1, 3);
				if (Game1.random.NextDouble() < 0.1 + who.team.AverageDailyLuck())
				{
					roe.Stack++;
				}
				if (Game1.random.NextDouble() < 0.1 + who.team.AverageDailyLuck())
				{
					roe.Stack *= 2;
				}
				treasures.Add(roe);
			}
		}
		if ((int)Game1.player.fishingLevel > 4 && Game1.player.stats.Get("FishingTreasures") > 2 && Game1.random.NextDouble() < 0.02 + ((!Game1.player.mailReceived.Contains("roeBookDropped")) ? ((double)Game1.player.stats.Get("FishingTreasures") * 0.001) : 0.001))
		{
			treasures.Add(ItemRegistry.Create("(O)Book_Roe"));
			Game1.player.mailReceived.Add("roeBookDropped");
		}

		return treasures;
    }

    public static Item mCreateFish(FishingRod ctx)
    {
	    //TAKEN AND MODIFIED FROM StardewValley.Tools.FishingRod.CreateFish
	    
	    Item fish = ctx.whichFish.CreateItemOrErrorItem(1, ctx.fishQuality);
	    if (!fish.HasTypeObject()) return fish;
	    if (fish.QualifiedItemId == GameLocation.CAROLINES_NECKLACE_ITEM_QID)
	    {
		    if (fish is Object obj)
		    {
			    obj.questItem.Value = true;
		    }
	    }
	    else if (ctx.numberOfFishCaught > 1 && fish.QualifiedItemId != "(O)79" && fish.QualifiedItemId != "(O)842")
	    {
		    fish.Stack = ctx.numberOfFishCaught;
	    }
	    return fish;
    }
}