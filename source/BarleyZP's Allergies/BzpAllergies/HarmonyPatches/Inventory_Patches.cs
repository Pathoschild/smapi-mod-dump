/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using BZP_Allergies.HarmonyPatches.UI;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using System.Text;

namespace BZP_Allergies.HarmonyPatches
{
    internal class Inventory_Patches
    {
        public static void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.getDescription)),
                postfix: new HarmonyMethod(typeof(Inventory_Patches), nameof(GetDescription_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Item), nameof(Item.canStackWith)),
                postfix: new HarmonyMethod(typeof(Inventory_Patches), nameof(CanStackWith_Postfix))
            );
        }

        public static void GetDescription_Postfix(StardewValley.Object __instance, ref string __result)
        {
            // note: this method gets called A LOT. This prefix needs to be efficient
            // 25 chars max a line
            try
            {
                string itemId = Traverse.Create(__instance).Property("ItemId").GetValue<string>();

                // is it the allergy teach book?
                bool isAllergyTeachBook = itemId == Constants.AllergyTeachBookId;
                bool alreadyRead = Game1.player.stats.Get(itemId) > 0;

                ISet<string> playerHas = AllergenManager.ModDataSetGet(Game1.player, Constants.ModDataHas);
                ISet<string> playerDiscovered = AllergenManager.ModDataSetGet(Game1.player, Constants.ModDataDiscovered);

                bool canDiscoverMore = playerDiscovered.Count < playerHas.Count;
                if (isAllergyTeachBook && alreadyRead && canDiscoverMore)
                {
                    ParsedItemData? originalBook = ItemRegistry.GetData("(O)" + itemId);
                    if (originalBook != null)
                    {
                        __result = originalBook.Description;
                    }
                    return;
                }

                // see if it has allergens in it
                ISet<string> allergens = AllergenManager.GetAllergensInObject(__instance);
                if (allergens.Count == 0) return;

                StringBuilder allergenText = new("\n" + ModEntry.Instance.Translation.Get("item-desc-prefix"));
                int currLineLen = allergenText.Length;

                int i = 0;
                foreach (string a in allergens)
                {
                    int len = a.Length;
                    if (currLineLen + len > 25)
                    {
                        allergenText.Append('\n');
                        currLineLen = 0;
                    }

                    allergenText.Append(AllergenManager.GetAllergenDisplayName(a));
                    currLineLen += len;

                    if (i < allergens.Count - 1)
                    {
                        allergenText.Append(", ");
                        currLineLen += 2;
                    }
                    i++;
                }
                __result += allergenText.ToString();
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(GetDescription_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        public static void CanStackWith_Postfix(ISalable other, Item __instance, ref bool __result)
        {
            try
            {
                // don't do any work if we know they can't stack already
                if (!__result) return;

                ISet<string> instanceAllergens = AllergenManager.GetAllergensInObject(__instance as StardewValley.Object);
                ISet<string> otherAllergens = AllergenManager.GetAllergensInObject(other as StardewValley.Object);

                if (instanceAllergens.Count != otherAllergens.Count)
                {
                    // different allergen count, so different allergens. can't stack
                    __result = false;
                    return;
                }

                // do they contain the same allergens?
                foreach (string a in instanceAllergens)
                {
                    if (!otherAllergens.Contains(a))
                    {
                        __result = false;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(CanStackWith_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}