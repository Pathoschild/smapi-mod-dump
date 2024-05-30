/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;
using System.Reflection.Emit;
using System.Reflection;
using StardewModdingAPI;

namespace BZP_Allergies.HarmonyPatches
{

    internal class CraftingCooking_Patches
    {
        public static ISet<string>? JustCookedWith;
        public static string? JustCookedWithName;

        public static void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingPage), "clickCraftingRecipe"),
                transpiler: new HarmonyMethod(typeof(CraftingCooking_Patches), nameof(ClickCraftingRecipe_Transpiler))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.cookedRecipe)),
                postfix: new HarmonyMethod(typeof(CraftingCooking_Patches), nameof(CookedRecipe_Postfix))
            );
        }

        public static void CookedRecipe_Postfix()
        {
            try
            {
                if (!ModEntry.Instance.Config.CookingReaction || JustCookedWith == null) return;

                foreach (string a in JustCookedWith)
                {
                    if (AllergenManager.FarmerIsAllergic(a))
                    {
                        Game1.player.applyBuff(AllergenManager.GetAllergicReactionBuff(JustCookedWithName!, "cook", ModEntry.Instance.Config.EatingDebuffLengthSeconds / 2));
                        AllergenManager.CheckForAllergiesToDiscover(Game1.player, JustCookedWith);
                        break;
                    }
                }

                JustCookedWith = null;
                JustCookedWithName = null;
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(CookedRecipe_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        public static IEnumerable<CodeInstruction> ClickCraftingRecipe_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // find the "createItem()" instruction
            // its result is in local var 1
            MethodInfo createItemMethod = AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.createItem));

            bool foundCreateItem = false;
            bool done = false;

            var inputInstrList = new List<CodeInstruction>(instructions);

            List<CodeInstruction> codes = new();
            for (int i = 0; i < inputInstrList.Count; i++)
            {
                var instr = inputInstrList[i];

                if (foundCreateItem && !done)
                {
                    // put the recipe on the stack
                    codes.Add(new CodeInstruction(OpCodes.Ldloc_0));

                    // put the item back on the stack with lodloc.1
                    codes.Add(new CodeInstruction(OpCodes.Ldloc_1));

                    // put this._materialContainers on the stack
                    // ldarg.0
                    codes.Add(new CodeInstruction(OpCodes.Ldarg_0));

                    // ldfld class(List<IInventory>) StardewValley.Menus.CraftingPage::_materialContainers
                    var _materialContainers = AccessTools.Field(typeof(CraftingPage), nameof(CraftingPage._materialContainers));
                    codes.Add(new CodeInstruction(OpCodes.Ldfld, _materialContainers));

                    // call AddPotentialAllergiesFromCraftingToItem
                    MethodInfo mine = AccessTools.Method(typeof(CraftingCooking_Patches), nameof(AddPotentialAllergiesFromCraftingToItem));
                    codes.Add(new CodeInstruction(OpCodes.Call, mine));

                    done = true;
                }

                if (instr.opcode == OpCodes.Callvirt && instr.operand is MethodInfo minfo && minfo == createItemMethod)
                {
                    foundCreateItem = true;
                    codes.Add(instr);
                    i++;
                    codes.Add(inputInstrList[i]);
                }

                else codes.Add(instr);
            }

            return codes.AsEnumerable();
        }

        public static void AddPotentialAllergiesFromCraftingToItem(CraftingRecipe recipe, Item crafted, List<IInventory> additionalMaterials)
        {
            if (crafted is null || crafted is not StardewValley.Object o || o.Edibility == -300) return;


            // copy the inventories
            Inventory playerItems = CopyInventory(Game1.player.Items);

            List<IInventory> additionalMaterialsCopy = new();
            if (additionalMaterials != null)
            {
                foreach (IInventory inv in additionalMaterials)
                {
                    if (inv == null)
                    {
                        additionalMaterialsCopy.Add(inv);
                        continue;
                    }
                    additionalMaterialsCopy.Add(CopyInventory(inv));
                }
            }

            // consume ingredients
            Dictionary<string, Item> beforeConsume = GetItemsInAllInventories(additionalMaterials);

            recipe.consumeIngredients(additionalMaterialsCopy);

            // see what was used
            Dictionary<string, Item> afterConsume = GetItemsInAllInventories(additionalMaterialsCopy);

            // subtract afterConsume amounts from beforeConsume to get the consumed materials
            List<Item> usedItems = new();
            foreach (var pair in beforeConsume)
            {
                
                int numAfter = 0;
                if (afterConsume.ContainsKey(pair.Key))
                {
                    numAfter = afterConsume[pair.Key].Stack;
                }

                if (pair.Value.Stack - numAfter > 0)
                {
                    // we used some of this item
                    usedItems.Add(pair.Value.getOne());
                }
            }

            
            // what allergens did we cook this with?
            crafted.modData[Constants.ModDataMadeWith] = "";
            foreach (Item item in usedItems)
            {
                if (item == null) continue;

                // what allergens does it have?
                ISet<string> allergens = AllergenManager.GetAllergensInObject(item as StardewValley.Object);
                foreach (string allergen in allergens)
                {
                    JustCookedWith ??= new HashSet<string>();  // there are some allergies
                    JustCookedWith.Add(allergen);
                    AllergenManager.ModDataSetAdd(crafted, Constants.ModDataMadeWith, allergen);
                }
            }

            // set static vars
            JustCookedWithName = crafted.DisplayName;

            // restore inventories
            Game1.player.Items.OverwriteWith(playerItems);

            if (additionalMaterials != null)
            {
                for (int i = 0; i < additionalMaterials.Count; i++)
                {
                    if (additionalMaterials[i] == null) continue;
                    additionalMaterials[i].OverwriteWith(additionalMaterialsCopy[i]);
                }
            }
        }

        private static Dictionary<string, Item> GetItemsInAllInventories(List<IInventory> additionalMaterials)
        {
            Dictionary<string, Item> result = new();
            if (additionalMaterials == null) return result;

            foreach (Item i in Game1.player.Items)
            {
                if (i == null) continue;
                Item copy = i.getOne();
                copy.Stack = i.Stack;
                result[i.ItemId] = copy;
            }

            foreach (IInventory inv in additionalMaterials)
            {
                if (inv is null) continue;
                foreach (Item i in inv)
                {
                    if (i == null) continue;
                    Item copy = i.getOne();
                    copy.Stack = i.Stack;
                    result[i.ItemId] = copy;
                }
            }

            return result;
        }

        private static Inventory CopyInventory(IInventory inventory)
        {
            Inventory result = new();

            foreach (Item i in inventory)
            {
                if (i is null)
                {
                    result.Add(i);
                    continue;
                }

                Item copy = i.getOne();
                copy.Stack = i.Stack;
                result.Add(copy);
            }


            return result;
        }
    }
}
