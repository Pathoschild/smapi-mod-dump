/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class PurchaseAnimalsMenuPatch
    {
        internal static FarmAnimal? animalBeingPurchased = null;
        internal static bool isOnFarm = false;
        internal static string purchaseAnimalMenuQuery = "";
        internal static PurchaseAnimalsMenu? purchaseAnimalsMenu;
        internal static bool firstTimeInNamingMenu = true;

        internal static void DrawPatch(PurchaseAnimalsMenu __instance, bool ___onFarm, bool ___namingAnimal, TextBox ___textBox, FarmAnimal ___animalBeingPurchased)
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
                else if (___onFarm && !___namingAnimal)
                {
                    firstTimeInNamingMenu = true;
                }
                else if (!___onFarm && !___namingAnimal)
                {
                    firstTimeInNamingMenu = true;
                    NarratePurchasingMenu(__instance);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void NarrateNamingMenu(PurchaseAnimalsMenu __instance, TextBox ___textBox, int x, int y)
        {
            string toSpeak = "";
            if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
            {
                toSpeak = "Cancel Button";
            }
            else if (__instance.doneNamingButton != null && __instance.doneNamingButton.containsPoint(x, y))
            {
                toSpeak = "OK Button";
            }
            else if (__instance.randomButton != null && __instance.randomButton.containsPoint(x, y))
            {
                toSpeak = "Random Name Button";
            }
            else if (__instance.textBoxCC != null && __instance.textBoxCC.containsPoint(x, y))
            {
                toSpeak = "Name Text Box";
                string? name = ___textBox.Text;
                if (name != null)
                    toSpeak = $"{toSpeak}, Value: {name}";
            }

            if (purchaseAnimalMenuQuery == toSpeak) return;

            purchaseAnimalMenuQuery = toSpeak;

            if (firstTimeInNamingMenu)
            {
                toSpeak = $"Enter the name of animal in the name text box. {toSpeak}";
                firstTimeInNamingMenu = false;
            }

            MainClass.ScreenReader.Say(toSpeak, true);
        }

        private static void NarratePurchasingMenu(PurchaseAnimalsMenu __instance)
        {
            if (__instance.hovered == null)
                return;

            string toSpeak = "";
            if (((StardewValley.Object)__instance.hovered.item).Type != null)
            {
                toSpeak = ((StardewValley.Object)__instance.hovered.item).Type;
            }
            else
            {
                string displayName = PurchaseAnimalsMenu.getAnimalTitle(__instance.hovered.hoverText);
                int price = __instance.hovered.item.salePrice();
                string description = PurchaseAnimalsMenu.getAnimalDescription(__instance.hovered.hoverText);

                toSpeak = $"{displayName}, Price: {price}g, Description: {description}";
            }

            if (purchaseAnimalMenuQuery == toSpeak) return;

            purchaseAnimalMenuQuery = toSpeak;
            MainClass.ScreenReader.Say(toSpeak, true);
        }

        internal static void Cleanup()
        {
            purchaseAnimalMenuQuery = "";
            firstTimeInNamingMenu = true;
            purchaseAnimalsMenu = null;
        }
    }
}
