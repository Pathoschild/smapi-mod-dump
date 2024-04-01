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
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Shops;

namespace GingerIslandStart.Additions;

public static class ResourceShop
{
    private static string Id => ModEntry.Id;
    private static double Mult => ModEntry.ShopMultiplier;
    private static readonly string ItemExtensionsId = ItemExtensions.ModEntry.Id;
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    
    public static ShopData Create()
    {
        var result = new ShopData
        {
            Owners = GetShopOwner(),
            SalableItemTags = new List<string>() {"category_minerals", "category_metal_resources", "category_gem"},
            Items = GetShopItems()
        };
            
        if(ModEntry.Config.FasterWeaponAccess)
            result.Items.AddRange(Weapons.GetShopData());
        
        return result;
    }

    private static List<ShopOwnerData> GetShopOwner()
    {
        var all = new List<ShopOwnerData>
        {
            new()
            {
                Condition = "LOCATION_NAME Here VolcanoDungeon0",
                Portrait = $"Mods/{Id}/Shop_Dwarf",
                Dialogues = DwarfDialogue(),
                RandomizeDialogueOnOpen = false,
                ClosedMessage = null,
                Id = "Dwarf",
                Name = null
            }
        };

        return all;
    }

    private static List<ShopDialogueData> DwarfDialogue()
    {
        return new List<ShopDialogueData>
        {
            new ShopDialogueData
            {
                Id = "Rare",
                Condition = "SYNCED_RANDOM day volcano_shop_dialogue .1",
                Dialogue = "[LocalizedText Strings\\StringsFromCSFiles:VolcanoShop4]"
            },
            new ShopDialogueData
            {
                Id = "Default",
                Condition = null,
                Dialogue = null,
                RandomDialogue = new List<string>
                {
                    "[LocalizedText Strings\\StringsFromCSFiles:VolcanoShop0]",
                    "[LocalizedText Strings\\StringsFromCSFiles:VolcanoShop2]",
                    "[LocalizedText Strings\\StringsFromCSFiles:VolcanoShop3]"
                }
            }
        };
    }
    
    private static List<ShopItemData> GetShopItems()
    {
        var allItems = new List<ShopItemData>();
        
        try
        {
            AddUpgrades("Axe", allItems);
            AddUpgrades("Hoe", allItems);
            AddUpgrades("Pickaxe", allItems);
        }
        catch (Exception e)
        {
            Log("Error: " + e);
            throw;
        }
        allItems.AddRange(WateringCans());

        var amt = ModEntry.GeneralDifficultyMultiplier switch
        {
            0.5 => 15,
            2 => 25,
            _ => 20
        };
        
        var shopItems = new List<ShopItemData>()
        {
            /*
             10 omni geode (x1 battery)
             2 Gold Bar && 50 Stone && 1 Diamond &&  (x1 geode crusher)
             2 taro tuber OR 2 ginger (x3 copper ore)
             1 cinder shard or 3 copper (x2 iron ore)
             ! 2 cinder, 3 mango/banana/pineapple, 3 iron (x1 gold ore)
             ! 3 gold OR 5 iron (x1 iridium)
             Amethyst (x2 iridium)
             emerald, ruby, or topaz (x4 iridium)
             5 wood (x1 hardwood, with a 2% that x2 wood → 1 hardwood)
             */
            
            //25 stone 20 ore
            new(){ Id = "Furnace", ItemId = "(BC)13", TradeItemId = "(O)390", TradeItemAmount = 25, Price = 3000, AvailableStock = 1, Condition = $"PLAYER_STAT Current copperFound {amt}", CustomFields = new Dictionary<string, string> {{ItemExtensions.Additions.ModKeys.ExtraTradesKey, "(O)378 20"}} },
            
            new(){ Id = "Battery", ItemId = "(O)787", TradeItemId = "(O)749", TradeItemAmount = 8, Price = 0},
            new(){ Id = "CopperOre_sap", ItemId = "(O)378", TradeItemId = "(O)92", TradeItemAmount = 6, Price = 0},
            new(){ Id = "CopperOre_ginger", ItemId = "(O)378", TradeItemId = "(O)829", TradeItemAmount = 2, Price = 0},
            new(){ Id = "IronOre_cinder", ItemId = "(O)380", TradeItemId = "(O)848", TradeItemAmount = 2, Price = 0},
            new(){ Id = "IronOre_copper", ItemId = "(O)380", TradeItemId = "(O)378", TradeItemAmount = 3, Price = 0},
            
            //only 2 of these at any time
            new(){ Id = "GoldOre_mango", ItemId = "(O)384", TradeItemId = "(O)834", TradeItemAmount = 3, Condition = "SYNCED_CHOICE day goldTrade 1 6 1 2", Price = 0},
            new(){ Id = "GoldOre_banana", ItemId = "(O)384", TradeItemId = "(O)91", TradeItemAmount = 3, Condition = "SYNCED_CHOICE day goldTrade 1 6 1 3", Price = 0},
            new(){ Id = "GoldOre_pineapple", ItemId = "(O)384", TradeItemId = "(O)832", TradeItemAmount = 3, Condition = "SYNCED_CHOICE day goldTrade 1 6 4 6", Price = 0},
            new(){ Id = "GoldOre_iron", ItemId = "(O)384", TradeItemId = "(O)380", TradeItemAmount = 3, Condition = "SYNCED_CHOICE day goldTrade 1 6 5 6", Price = 0},

            new(){ Id = "IridiumOre_gold", ItemId = "(O)386", TradeItemId = "(O)384", TradeItemAmount = 3, Price = 0},
            
            //only 1 of these at any time
            new(){ Id = "IridiumOre_emerald", ItemId = "(O)386", TradeItemId = "(O)60", TradeItemAmount = 1, Condition = "SYNCED_CHOICE day iridiumTrade 1 6 1 2", Price = 0},
            new(){ Id = "IridiumOre_ruby", ItemId = "(O)386", TradeItemId = "(O)64", TradeItemAmount = 1, Condition = "SYNCED_CHOICE day iridiumTrade 1 6 3 4", Price = 0},
            new(){ Id = "IridiumOre_topaz", ItemId = "(O)386", TradeItemId = "(O)68", TradeItemAmount = 1, Condition = "SYNCED_CHOICE day iridiumTrade 1 6 5 6", Price = 0},
        };

        foreach (var item in shopItems)
        {
            if (item.Price > 1)
                item.Price = (int)(item.Price * Mult);
            
            
            if(item.TradeItemAmount > 1)
                item.TradeItemAmount = (int)(item.TradeItemAmount * Mult);
        }

        allItems.AddRange(shopItems);
        
        if(Mult.Equals(0.5))
            allItems.Add(new ShopItemData { Id = "WoodToHardwood", ItemId = "(O)709", TradeItemId = "(O)388", TradeItemAmount = Game1.random.Next(6,10), Price = 0});
        
        return allItems;
    }

    private static List<ShopItemData> WateringCans()
    {
        const string t = "WateringCan";
        var list = new List<ShopItemData>();
        
        list.Add(new ShopItemData
        {
            Id = $"Tool_Copper{t}", ItemId = $"(T)Copper{t}", TradeItemId = "(O)334", TradeItemAmount = 5,
            Condition = $"{ItemExtensionsId}_InInventory Current (T){t}", Price = 2000, MaxStack = 1, MaxItems = 1, AvailableStock = 1, CustomFields = new Dictionary<string, string>{{ ItemExtensions.Additions.ModKeys.ExtraTradesKey, $"(T){t} 1"}}
        });

        list.Add(new ShopItemData
        {
            Id = $"Tool_Iron{t}", ItemId = $"(T)Steel{t}", TradeItemId = "(O)335", TradeItemAmount = 5,
            Condition = $"{ItemExtensionsId}_InInventory Current (T)Copper{t}", Price = 5000, MaxStack = 1, MaxItems = 1, AvailableStock = 1, CustomFields = new Dictionary<string, string>{{ ItemExtensions.Additions.ModKeys.ExtraTradesKey, $"(T)Copper{t} 1"}}
        });
        
        list.Add(new ShopItemData
        {
            Id = $"Tool_Gold{t}", ItemId = $"(T)Gold{t}", TradeItemId = "(O)336", TradeItemAmount = 5,
            Condition = $"{ItemExtensionsId}_InInventory Current (T)Steel{t}", Price = 10000, MaxStack = 1, MaxItems = 1, AvailableStock = 1, CustomFields = new Dictionary<string, string>{{ ItemExtensions.Additions.ModKeys.ExtraTradesKey, $"(T)Steel{t} 1"}}
        });
        
        list.Add(new ShopItemData
        {
            Id = $"Tool_Iridium{t}", ItemId = $"(T)Iridium{t}", TradeItemId = "(O)337", TradeItemAmount = 5,
            Condition = $"{ItemExtensionsId}_InInventory Current (T)Gold{t}", Price = 25000, MaxStack = 1, MaxItems = 1, AvailableStock = 1, CustomFields = new Dictionary<string, string>{{ ItemExtensions.Additions.ModKeys.ExtraTradesKey, $"(T)Gold{t} 1"}}
        });
        
        return list;
    }

    private static void AddUpgrades(string t, ICollection<ShopItemData> list)
    {
        #if DEBUG
        Log($"Making {t}");
        #endif

        const string extraTradesKey = "mistyspring.ItemExtensions/ExtraTrades";
        
        list.Add(new ShopItemData
        {
            Id = $"Tool_Copper{t}", ItemId = $"(T)Copper{t}", TradeItemId = "(O)334", TradeItemAmount = 5,
            Condition = $"{ItemExtensionsId}_ToolUpgrade Current {t} 0 0 false", Price = 2000, MaxStack = 1, MaxItems = 1, AvailableStock = 1, CustomFields = new Dictionary<string, string> {{ extraTradesKey, $"(T){t} 1"}}
        });

        list.Add(new ShopItemData
        {
            Id = $"Tool_Iron{t}", ItemId = $"(T)Steel{t}", TradeItemId = "(O)335", TradeItemAmount = 5,
            Condition = $"{ItemExtensionsId}_ToolUpgrade Current {t} 1 1 false", Price = 5000, MaxStack = 1, MaxItems = 1, AvailableStock = 1, CustomFields = new Dictionary<string, string>{{ extraTradesKey, $"(T)Copper{t} 1"}}
        });
        
        list.Add(new ShopItemData
        {
            Id = $"Tool_Gold{t}", ItemId = $"(T)Gold{t}", TradeItemId = "(O)336", TradeItemAmount = 5,
            Condition = $"{ItemExtensionsId}_ToolUpgrade Current {t} 2 2 false", Price = 10000, MaxStack = 1, MaxItems = 1, AvailableStock = 1, CustomFields = new Dictionary<string, string>{{ extraTradesKey, $"(T)Steel{t} 1"}}
        });
        
        list.Add(new ShopItemData
        {
            Id = $"Tool_Iridium{t}", ItemId = $"(T)Iridium{t}", TradeItemId = "(O)337", TradeItemAmount = 5,
            Condition = $"{ItemExtensionsId}_ToolUpgrade Current {t} 3 3 false", Price = 25000, MaxStack = 1, MaxItems = 1, AvailableStock = 1, CustomFields = new Dictionary<string, string>{{ extraTradesKey, $"(T)Gold{t} 1"}}
        });
    }

    public static Dictionary<string,ItemData> CreateBehaviors()
    {
        var result = new Dictionary<string, ItemData>();
        var upgrades = new[]{"Axe", "Hoe", "Pickaxe", "WateringCan"};
        foreach (var name in upgrades)
        {
            var data = new ItemData
            {
                OnPurchase = new OnBehavior
                {
                    RemoveItems = new Dictionary<string, int> { { $"(T){name}", 1 } },
                    Conditions = "PLAYER_LOCATION_CONTEXT Current Island"
                }
            };

            result.Add($"(T)Copper{name}", data);

            var data2 = new ItemData
            {
                OnPurchase = new OnBehavior
                {
                    RemoveItems = new Dictionary<string, int> { { $"(T)Copper{name}", 1 } },
                    Conditions = "PLAYER_LOCATION_CONTEXT Current Island"
                }
            };
            result.Add($"(T)Iron{name}", data2);
            
            var data3 = new ItemData
            {
                OnPurchase = new OnBehavior
                {
                    RemoveItems = new Dictionary<string, int> { { $"(T)Iron{name}", 1 } },
                    Conditions = "PLAYER_LOCATION_CONTEXT Current Island"
                }
            };
            result.Add($"(T)Gold{name}", data3);
            
            var data4 = new ItemData
            {
                OnPurchase = new OnBehavior
                {
                    RemoveItems = new Dictionary<string, int> { { $"(T)Gold{name}", 1 } },
                    Conditions = "PLAYER_LOCATION_CONTEXT Current Island"
                }
            };
            result.Add($"(T)Iridium{name}", data4);
        }
        return result;
    }
/*
    public static Dictionary<string, List<ExtraItems>> CreateExtraRequirements(bool volcanoShop)
    {
        var result = new Dictionary<string, List<ExtraItems>>();
        if (volcanoShop)
        {
            var geodeCrusherExtras = new List<ExtraItems>
            {
                new("(O)390", 50), 
                new("(O)72", 1)
            };
            result.Add("(BC)182", geodeCrusherExtras); //50 Stone && 1 Diamond
        }
        else
        {
            var furnaceExtras = new List<ExtraItems>
            {
                new("(O)378", 20), 
            };
            result.Add("(BC)13", furnaceExtras); //20 copper
        }

        return result;
    }*/
}