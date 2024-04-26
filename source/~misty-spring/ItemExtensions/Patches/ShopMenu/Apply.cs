/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Text;
using HarmonyLib;
using ItemExtensions.Models.Internal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemExtensions.Patches;

public partial class ShopMenuPatches
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    private static IReflectionHelper Reflection => ModEntry.Help.Reflection;

    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(ShopMenuPatches)}\": prefixing SDV method \"ShopMenu.Initialize\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(ShopMenu), "Initialize"),
            postfix: new HarmonyMethod(typeof(ShopMenuPatches), nameof(Post_Initialize))
        );
        
        Log($"Applying Harmony patch \"{nameof(ShopMenuPatches)}\": prefixing SDV method \"ShopMenu.Initialize\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(ShopMenu), "AddForSale"),
            postfix: new HarmonyMethod(typeof(ShopMenuPatches), nameof(Post_AddForSale))
        );
        
        Log($"Applying Harmony patch \"{nameof(ShopMenuPatches)}\": postfixing SDV method \"ShopMenu.cleanupBeforeExit\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(ShopMenu), "cleanupBeforeExit"),
            postfix: new HarmonyMethod(typeof(ShopMenuPatches), nameof(Post_cleanupBeforeExit))
        );
        
       var types = new[]
       {
           typeof(SpriteBatch),
           typeof(StringBuilder),
           typeof(SpriteFont),
           typeof(int),
           typeof(int),
           typeof(int),
           typeof(string),
           typeof(int),
           typeof(string[]),
           typeof(Item),
           typeof(int),
           typeof(string),
           typeof(int),
           typeof(int),
           typeof(int),
           typeof(float),
           typeof(CraftingRecipe),
           typeof(IList<Item>),
           typeof(Texture2D),
           typeof(Rectangle?),
           typeof(Color?),
           typeof(Color?),
           typeof(float),
           typeof(int),
           typeof(int)
       };
      
        Log($"Applying Harmony patch \"{nameof(ShopMenuPatches)}\": postfixing SDV method \"IClickableMenu.drawHoverText\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.drawHoverText), types),
            postfix: new HarmonyMethod(typeof(ShopMenuPatches), nameof(Post_drawHoverText))
        );
        
        Log($"Applying Harmony patch \"{nameof(ShopMenuPatches)}\": prefixing SDV method \"ShopMenu.receiveLeftClick\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.receiveLeftClick)),
            prefix: new HarmonyMethod(typeof(ShopMenuPatches), nameof(Pre_receiveLeftClick))
        );
        
        Log($"Applying Harmony patch \"{nameof(ShopMenuPatches)}\": prefixing SDV method \"ShopMenu.receiveLeftClick\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.receiveRightClick)),
            prefix: new HarmonyMethod(typeof(ShopMenuPatches), nameof(Pre_receiveLeftClick))
        );
    }

    internal static void Post_Initialize(ShopMenu __instance, int currency, Func<ISalable, Farmer, int, bool> onPurchase, Func<ISalable, bool> onSell, bool playOpenSound)
    {
        #if DEBUG
        Log($"Initializing shop with {ExtraBySalable?.Count} extra trade requirements.");
        #endif
    }
    
    internal static void Post_cleanupBeforeExit()
    {
        #if DEBUG
        Log("Resetting extra trades dictionary...");
        #endif
        
        ExtraBySalable = new Dictionary<ISalable, List<ExtraTrade>>();
    }
    
    /// <summary>
    /// Adds salable items.
    /// </summary>
    /// <param name="__instance"></param>
    /// <param name="item"></param>
    /// <param name="stock"></param>
    private static void Post_AddForSale(ShopMenu __instance,ISalable item, ItemStockInformation? stock = null)
    {
        try
        {
            //if null, make
            ExtraBySalable ??= new Dictionary<ISalable, List<ExtraTrade>>();

            //get match from data
            var match = FindMatch(__instance, item, stock);

            //if none was found
            if (match is null)
            {
#if DEBUG
                Log("No match found.");
#endif
                return;
            }

            //if couldn't get right info
            if (ExtraTrade.TryParse(match, out var extras) == false)
                return;

            ExtraBySalable.Add(item, extras);
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }

    private static string FindMatch(ShopMenu menu, ISalable item, ItemStockInformation? stock = null)
    {
        if (string.IsNullOrWhiteSpace(menu.ShopId))
        {
            Log("Shop Id seems to be null. Skipping...");
            return null;
        }
        
        if (DataLoader.Shops(Game1.content).TryGetValue(menu.ShopId, out var shopData) == false)
        {
            Log("Shop Id not found. Skipping...");
            return null;
        }

        var identifier = stock != null ? stock.Value.SyncedKey : item.QualifiedItemId;
        
        foreach (var dataItem in shopData.Items)
        {
            /*
            #if DEBUG
            Log($"Checking {dataItem.Id} (for {identifier})");
            #endif*/

            if (stock is null)
            {
                #if DEBUG
                Log("No stock data found.", LogLevel.Warn);
                #endif
                return null;
            }
            
            //compare stocks' Ids
            if(dataItem.Id.Equals(stock.Value.SyncedKey) == false)
                continue;

            if (dataItem.CustomFields is null || dataItem.CustomFields.Any() == false)
            {
                #if DEBUG
                Log("Item seems to have no custom fields. Skipping");
                #endif
                return null;
            }
            
            //if stock data -somehow- has no data, ignore
            if (dataItem.CustomFields.TryGetValue(Additions.ModKeys.ExtraTradesKey, out var trades))
            {
                #if DEBUG
                Log($"Found data at {dataItem.Id}");
                #endif
                return trades;
            }
        }

        return null;
    }

    private static bool InDictionary(Item hoveredItem, out List<ExtraTrade> list)
    {
        list = null;
        
        if (hoveredItem is null)
            return false;
        
        //prioritize bySalable check
        foreach (var pair in ExtraBySalable)
        {
            if (pair.Key?.QualifiedItemId != hoveredItem.QualifiedItemId)
                continue;

            if (pair.Key?.IsRecipe != hoveredItem.IsRecipe)
                continue;
            
            if(pair.Key?.Quality != hoveredItem.Quality)
                continue;

            if (pair.Key?.IsInfiniteStock() != hoveredItem.IsInfiniteStock())
                continue;
            
            if(pair.Key?.Stack != hoveredItem.Stack)
                continue;

            list = pair.Value;
            return true;
        }

        return false;
    }
}