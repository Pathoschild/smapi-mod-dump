/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.Inventories;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ResourceStorage
{
    public partial class ModEntry
    {
        public static bool Inventory_ReduceId_Prefix(Inventory __instance, string itemId, ref int count)
        {
            if (!Config.ModEnabled || !Config.AutoUse)
                return true;

            if(!TryGetInventoryOwner(__instance, out Farmer farmer))
            {
                return true;
            }

            count += (int)ModifyResourceLevel(farmer, ItemRegistry.QualifyItemId(itemId), -count, auto: true);
            return count > 0;
        }
        public static void Inventory_CountId_Postfix(Inventory __instance, string itemId, ref int __result)
        {
            if (!Config.ModEnabled || !Config.AutoUse || !ModEntry.TryGetInventoryOwner(__instance, out Farmer farmer))
                return;

            __result += (int)GetResourceAmount(farmer, ItemRegistry.QualifyItemId(itemId));
        }

        public static void Inventory_ContainsId_Postfix(Inventory __instance, string itemId, ref bool __result)
        {
            if (__result || !Config.ModEnabled || !Config.AutoUse || !TryGetInventoryOwner(__instance, out Farmer farmer))
                return;

            __result = (int)GetResourceAmount(farmer, ItemRegistry.QualifyItemId(itemId)) > 0;
        }

        public static void Inventory_ContainsId2_Postfix(Inventory __instance, string itemId, int minimum, ref bool __result)
        {
            if (__result || !Config.ModEnabled || !Config.AutoUse || !TryGetInventoryOwner(__instance, out Farmer farmer))
                return;

            __result = (int)GetResourceAmount(farmer, ItemRegistry.QualifyItemId(itemId)) >= minimum;
        }

        public static void Inventory_GetById_Postfix(Inventory __instance, string itemId, ref IEnumerable<Item> __result)
        {
            if (!Config.ModEnabled || !Config.AutoUse || !TryGetInventoryOwner(__instance, out Farmer farmer))
            {
                return;
            }

            __result.Append(ItemRegistry.Create<Item>(itemId, (int)GetResourceAmount(farmer, ItemRegistry.QualifyItemId(itemId))));
        }

        public static bool Farmer_addItemToInventory_Prefix(Farmer __instance, Item item)
        {
            if (!Config.ModEnabled || Game1.activeClickableMenu is ResourceMenu || item is not Object || !CanStore(item as Object))
                return true;

            long countAdded = ModifyResourceLevel(__instance, item.QualifiedItemId, item.Stack, auto: true);

            if(countAdded > 0)
            {
                __instance.OnItemReceived(item, (int)countAdded, null, true);
                return false;
            }

            return true;
        }

        // Transpiled becaues it was getting inlined
        public static IEnumerable<CodeInstruction> Farmer_getItemCount_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            SMonitor.Log($"Transpiling Farmer.getItemCount");
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count-2; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i + 2].opcode == OpCodes.Call && codes[i + 2].operand is MethodInfo info && info == AccessTools.Method(typeof(Farmer), nameof(Farmer.getItemCountInList)))
                {
                    SMonitor.Log("Replacing default method");
                    codes[i + 2].opcode = OpCodes.Call;
                    codes[i + 2].operand = AccessTools.Method(typeof(ModEntry), nameof(Farmer_GetItemCountTranspilerMethod));
                    break;
                }
            }

            return codes.AsEnumerable();
        }

        // The old postfix before we moved to a transpiler
        /*
        public static void Farmer_getItemCount_Postfix(Farmer __instance, string itemId, ref int __result)
        {
            if (!Config.ModEnabled || !Config.AutoUse)
                return;

            __result += GetMatchesForCrafting(__instance, itemId);
        }
        */

        public static void Farmer_couldInventoryAcceptThisItem_Postfix(Farmer __instance, Item item, ref bool __result)
        {
            if (!Config.ModEnabled || __result || item is not Object || !CanStore(item as Object))
                return;

            if (GetResourceAmount(__instance, item.QualifiedItemId) > 0 || CanAutoStore(item.QualifiedItemId))
                __result = true;
        }

        public static void Farmer_couldInventoryAcceptThisItem2_Postfix(Farmer __instance, string id, int quality, ref bool __result)
        {
            if (!Config.ModEnabled || __result || quality > 0)
                return;

            string qualifiedId = ItemRegistry.QualifyItemId(id);
            if (GetResourceAmount(__instance, qualifiedId) > 0 || CanAutoStore(qualifiedId))
                __result = true;
        }

        // Watch for inlining
        public static bool Object_ConsumeInventoryItem_Prefix(Farmer who, Item drop_in, ref int amount)
        {
            if (!Config.ModEnabled || !Config.AutoUse)
                return true;

            amount += (int)ModifyResourceLevel(who, drop_in.QualifiedItemId, -amount, auto: true);
            return amount > 0;
        }

        public static void CraftingRecipe_ConsumeAdditionalIngredientsPrefix(ref List<KeyValuePair<string, int>> additionalRecipeItems)
        {
            if (!Config.ModEnabled || !Config.AutoUse)
                return;
            for (int i = 0; i < additionalRecipeItems.Count; i++)
            {
                additionalRecipeItems[i] = new KeyValuePair<string, int>(additionalRecipeItems[i].Key, additionalRecipeItems[i].Value - ConsumeItemsForCrafting(Game1.player, additionalRecipeItems[i].Key, additionalRecipeItems[i].Value));
            }
        }

        public static IEnumerable<CodeInstruction> CraftingRecipe_getCraftableCount_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            SMonitor.Log($"Transpiling CraftingRecipe.getCraftableCount");
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldloc_3 && codes[i + 1].opcode == OpCodes.Ldloc_S && codes[i + 2].opcode == OpCodes.Div)
                {
                    SMonitor.Log($"adding method to increase ingredient count");
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(AddIngredientAmount))));
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldloc, (byte)4));
                }
            }

            return codes.AsEnumerable();
        }
        public static void CraftingRecipe_consumeIngredients_Prefix(CraftingRecipe __instance, ref Dictionary<string, int> __state)
        {
            if (!Config.ModEnabled || !Config.AutoUse)
                return;

            __state = __instance.recipeList;
            Dictionary<string, int> dict = new();
            foreach(var s in __state)
            {
                int amount = s.Value - ConsumeItemsForCrafting(Game1.player, s.Key, s.Value);
                if (amount <= 0)
                    continue;
                dict.Add(s.Key, amount);
            }
            __instance.recipeList = dict;
        }

        public static void CraftingRecipe_consumeIngredients_Postfix(CraftingRecipe __instance, ref Dictionary<string, int> __state)
        {
            if (!Config.ModEnabled || !Config.AutoUse)
                return;
            __instance.recipeList = __state;
        }

        public static void GameMenu_Constructor_Postfix(GameMenu __instance)
        {
            if (!Config.ModEnabled)
                return;
            gameMenu.Value = null;
            SetupResourceButton(__instance);
        }

        public static void InventoryPage_Constructor_Postfix(InventoryPage __instance)
        {
            if (!Config.ModEnabled)
                return;

            if (__instance.organizeButton is not null)
                __instance.organizeButton.downNeighborID = 42999;

            if (__instance.trashCan is not null)
                __instance.trashCan.upNeighborID = 42999;
        }

        public static void InventoryPage_draw_Prefix(SpriteBatch b)
        {
            if (!Config.ModEnabled || Game1.activeClickableMenu is not GameMenu menu)
                return;
            SetupResourceButton(menu); // Update the button's bounds
            resourceButton.Value.draw(b);
        }

        public static bool InventoryPage_performHoverAction_Prefix(ref string ___hoverText, int x, int y)
        {
            if (!Config.ModEnabled || Game1.activeClickableMenu is not GameMenu)
                return true;

            if (resourceButton.Value.containsPoint(x, y))
            {
                ___hoverText = resourceButton.Value.hoverText;
                return false;
            }
            return true;
        }

        public static bool InventoryPage_receiveKeyPressPrefix(InventoryPage __instance, Keys key, ref string ___hoverText)
        {
            if (!Config.ModEnabled || Game1.activeClickableMenu is not GameMenu)
                return true;
            if (SButtonExtensions.ToSButton(key) == Config.ResourcesKey)
            {
                ___hoverText = "";
                Game1.playSound("bigSelect");
                gameMenu.Value = Game1.activeClickableMenu as GameMenu;
                Game1.activeClickableMenu = new ResourceMenu();
                return false;
            }
            return true;
        }

        public static bool InventoryPage_receiveGamePadButton_Prefix(InventoryPage __instance, Buttons b, ref string ___hoverText)
        {
            if (!Config.ModEnabled || Game1.activeClickableMenu is not GameMenu)
                return true;
            if (SButtonExtensions.ToSButton(b) == Config.ResourcesKey)
            {
                ___hoverText = "";
                Game1.playSound("bigSelect");
                gameMenu.Value = Game1.activeClickableMenu as GameMenu;
                Game1.activeClickableMenu = new ResourceMenu();
                return false;
            }
            return true;
        }

        public static bool InventoryPage_receiveLeftClick_Prefix(InventoryPage __instance, ref string ___hoverText, int x, int y)
        {
            if (!Config.ModEnabled || Game1.activeClickableMenu is not GameMenu)
                return true;
            if (resourceButton.Value.containsPoint(x, y))
            {
                if (Game1.player.CursorSlotItem is Object obj)
                {
                    if (CanStore(obj))
                    {
                        Game1.playSound("Ship");
                        ModifyResourceLevel(Game1.player, obj.QualifiedItemId, Game1.player.CursorSlotItem.Stack, auto: false);
                        Game1.player.CursorSlotItem = null;
                    }
                }
                else
                {
                    ___hoverText = "";
                    Game1.playSound("bigSelect");
                    gameMenu.Value = Game1.activeClickableMenu as GameMenu;
                    Game1.activeClickableMenu = new ResourceMenu();

                }
                return false;
            }
            return true;
        }

        public static void IClickableMenu_populateClickableComponentList_Postfix(IClickableMenu __instance)
        {
            if (!Config.ModEnabled || __instance is not InventoryPage)
                return;

            if (resourceButton.Value is null)
                SetupResourceButton(__instance);

            __instance.allClickableComponents.Add(resourceButton.Value);
        }
    }
}