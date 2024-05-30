/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using HarmonyLib;
using stardew_access.Translation;
using StardewValley;
using StardewValley.GameData.FarmAnimals;
using StardewValley.Menus;
using StardewValley.TokenizableStrings;

namespace stardew_access.Patches;

internal class PurchaseAnimalsMenuPatch : IPatch
{
    internal static FarmAnimal? animalBeingPurchased = null;
    internal static bool isOnFarm = false;
    internal static PurchaseAnimalsMenu? purchaseAnimalsMenu;
    
    private static bool firstTimeInNamingMenu = true;

    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(PurchaseAnimalsMenu), "draw"),
            prefix: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.DrawPatch))
        );
    }

    private static void DrawPatch(PurchaseAnimalsMenu __instance, bool ___onFarm, bool ___namingAnimal, TextBox ___textBox, FarmAnimal ___animalBeingPurchased)
    {
        try
        {
            if (TextBoxPatch.IsAnyTextBoxActive) return;

            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
            purchaseAnimalsMenu = __instance;
            isOnFarm = ___onFarm;
            animalBeingPurchased = ___animalBeingPurchased;

            if (___onFarm && ___namingAnimal)
            {
                NarrateNamingMenu(__instance, ___textBox, x, y);
            }
            else if (___onFarm && !___namingAnimal && !Game1.IsFading())
            {
                firstTimeInNamingMenu = true;
	        string selectBuildingPrompt = Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11355", animalBeingPurchased.displayHouse, animalBeingPurchased.displayType);
                MainClass.ScreenReader.SayWithMenuChecker(selectBuildingPrompt, true);
            }
            else if (!___onFarm && !___namingAnimal)
            {
                firstTimeInNamingMenu = true;
                NarratePurchasingMenu(__instance);
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in purchase animal menu patch:\n{e.Message}\n{e.StackTrace}");
        }
    }

    private static void NarrateNamingMenu(PurchaseAnimalsMenu __instance, TextBox ___textBox, int x, int y)
    {
        string translationKey = "";
        object? translationTokens = null;
        if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
        {
            translationKey = "common-ui-cancel_button";
        }
        else if (__instance.doneNamingButton != null && __instance.doneNamingButton.containsPoint(x, y))
        {
            translationKey = "common-ui-ok_button";
        }
        else if (__instance.randomButton != null && __instance.randomButton.containsPoint(x, y))
        {
            translationKey = "menu-purchase_animal-random_name_button";
        }
        else if (__instance.textBoxCC != null && __instance.textBoxCC.containsPoint(x, y))
        {
            translationKey = "menu-purchase_animal-animal_name_text_box";
            translationTokens = new
            {
                value = string.IsNullOrEmpty(___textBox.Text) ? "null" : ___textBox.Text
            };
        }

        if (firstTimeInNamingMenu)
        {
            MainClass.ScreenReader.MenuPrefixNoQueryText = Translator.Instance.Translate("menu-purchase_animal-first_time_in_menu_info", TranslationCategory.Menu);
            firstTimeInNamingMenu = false;
        }

        MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens);
    }

    private static void NarratePurchasingMenu(PurchaseAnimalsMenu __instance)
    {
        if (__instance.hovered?.item == null) return;

        string translationKey = "";
        object? translationTokens = null;
        if (((StardewValley.Object)__instance.hovered.item).Type != null)
        {
            translationKey = ((StardewValley.Object)__instance.hovered.item).Type;
        }
        else if (Game1.farmAnimalData.TryGetValue(__instance.hovered.item!.Name, out FarmAnimalData? farmAnimalData) && farmAnimalData != null)
        {
            translationKey = "menu-purchase_animal-animal_info";
            translationTokens = new
            {
                name = FarmAnimal.GetDisplayName(__instance.hovered.hoverText, forShop: true),
                price = __instance.hovered.item.salePrice(),
                description = TokenParser.ParseText(farmAnimalData.ShopDescription)
            };
        }

        MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens);
    }

    internal static void Cleanup()
    {
        firstTimeInNamingMenu = true;
        purchaseAnimalsMenu = null;
    }
}
