/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Shops;

namespace GingerIslandStart.Additions;

public static class PirateShop
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
            SalableItemTags = new List<string>() {"category_meat", "category_sell_at_pierres_and_marnies"},
            Items = GetShopItems()
        };
        
        return result;
    }

    public static ShopData CreateRecoveryShop()
    {
        return new ShopData()
        {
            Owners = GetShopOwner(),
            SalableItemTags = new List<string>() {"category_meat", "category_sell_at_pierres_and_marnies"},
            Items = new List<ShopItemData>
            {
                new() { Id = "ITEMS_LOST_ON_DEATH", ItemId = "ITEMS_LOST_ON_DEATH" }
            }
        };
    }

    private static List<ShopOwnerData> GetShopOwner()
    {
        var all = new List<ShopOwnerData>
        {
            new()
            {
                Portrait = $"Mods/{Id}/Shop_Pirate",
                Dialogues = PirateDialogue(),
                RandomizeDialogueOnOpen = false,
                ClosedMessage = null,
                Id = "Pirate",
                Name = null
            }
        };

        return all;
    }

    private static List<ShopDialogueData> PirateDialogue()
    {
        return new List<ShopDialogueData>
        {
            new ShopDialogueData
            {
                Id = "Rare",
                Condition = "SYNCED_RANDOM day volcano_shop_dialogue .1",
                Dialogue = "[LocalizedText Strings\\StringsFromMaps:Pirates6]"
            },
            new ShopDialogueData
            {
                Id = "Default",
                Condition = null,
                Dialogue = null,
                RandomDialogue = new List<string>
                {
                    GetTrimmedDialogue("Strings\\StringsFromMaps", "Pirates5", '#',0),
                    GetTrimmedDialogue("Strings\\StringsFromMaps", "Pirates4"),
                    GetTrimmedDialogue("Strings\\StringsFromMaps", "Pirates8")
                }
            }
        };
    }

    private static string GetTrimmedDialogue(string file, string key, char splitBy = char.MinValue, int secondsplit = -1)
    {
        var raw = Game1.content.LoadString($"{file}:{key}");
        var part = ArgUtility.SplitBySpaceAndGet(raw, 0);

        if (part is null)
            throw new ContentLoadException();

        part += ' ';

        var fixedString = raw.Replace(part, "");

        if (splitBy == char.MinValue || secondsplit == -1)
            return fixedString;
        
        return fixedString.Split(splitBy)[secondsplit];
    }

    private static List<ShopItemData> GetShopItems()
    {
        var shopItems = new List<ShopItemData>
        {
            Weapons.Random()
        };

        shopItems.AddRange(Weapons.GetShopData());
        
        foreach (var item in shopItems)
        {
            if (item.Price > 1)
                item.Price = (int)(item.Price * Mult);
            
            
            if(item.TradeItemAmount > 1)
                item.TradeItemAmount = (int)(item.TradeItemAmount * Mult);
        }
        
        return shopItems;
    }
}