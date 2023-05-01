/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using stardew_access.Features;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    // These patches are global, i.e. work on every menus
    internal class IClickableMenuPatch
    {
        internal static void DrawHoverTextPatch(string? text, int moneyAmountToDisplayAtBottom = -1, string? boldTitleText = null, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1, string[]? buffIconsToDisplay = null, Item? hoveredItem = null, CraftingRecipe? craftingIngredients = null)
        {
            try
            {
                #region Skip narrating hover text for certain menus
                if (Game1.activeClickableMenu is TitleMenu && !(((TitleMenu)Game1.activeClickableMenu).GetChildMenu() is CharacterCustomization))
                    return;
                else if (Game1.activeClickableMenu is LetterViewerMenu || Game1.activeClickableMenu is QuestLog)
                    return;
                else if (Game1.activeClickableMenu is Billboard)
                    return;
                else if (Game1.activeClickableMenu is GeodeMenu)
                    return;
                else if (Game1.activeClickableMenu is GameMenu && ((GameMenu)Game1.activeClickableMenu).GetCurrentPage() is InventoryPage)
                    return;
                else if (Game1.activeClickableMenu is GameMenu && ((GameMenu)Game1.activeClickableMenu).GetCurrentPage() is CraftingPage)
                    return;
                else if (Game1.activeClickableMenu is GameMenu && ((GameMenu)Game1.activeClickableMenu).GetCurrentPage() is OptionsPage)
                    return;
                else if (Game1.activeClickableMenu is GameMenu && ((GameMenu)Game1.activeClickableMenu).GetCurrentPage() is ExitPage)
                    return;
                else if (Game1.activeClickableMenu is GameMenu && ((GameMenu)Game1.activeClickableMenu).GetCurrentPage() is SocialPage)
                    return;
                else if (Game1.activeClickableMenu is ItemGrabMenu)
                    return;
                else if (Game1.activeClickableMenu is ShopMenu)
                    return;
                else if (Game1.activeClickableMenu is ConfirmationDialog)
                    return;
                else if (Game1.activeClickableMenu is JunimoNoteMenu)
                    return;
                else if (Game1.activeClickableMenu is CarpenterMenu)
                    return;
                else if (Game1.activeClickableMenu is PurchaseAnimalsMenu)
                    return;
                else if (Game1.activeClickableMenu is CraftingPage)
                    return;
                else if (Game1.activeClickableMenu is AnimalQueryMenu)
                    return;
                else if (Game1.activeClickableMenu is ConfirmationDialog)
                    return;
                else if (Game1.activeClickableMenu is ReadyCheckDialog)
                    return;
                else if (Game1.activeClickableMenu is JojaCDMenu)
                    return;
                else if (Game1.activeClickableMenu is TailoringMenu)
                    return;
                else if (Game1.activeClickableMenu is PondQueryMenu)
                    return;
                else if (Game1.activeClickableMenu is ForgeMenu)
                    return;
                else if (Game1.activeClickableMenu is ItemListMenu)
                    return;
                else if (Game1.activeClickableMenu is FieldOfficeMenu)
                    return;
                else if (Game1.activeClickableMenu is MuseumMenu)
                    return;
                #endregion

                string toSpeak = " ";

                #region Add item count before title
                if (hoveredItem != null && hoveredItem.HasBeenInInventory)
                {
                    int count = hoveredItem.Stack;
                    if (count > 1)
                        toSpeak = $"{toSpeak} {count} ";
                }
                #endregion

                #region Add title if any
                if (boldTitleText != null)
                    toSpeak = $"{toSpeak} {boldTitleText}\n";
                #endregion

                #region Add quality of item
                if (hoveredItem is StardewValley.Object && ((StardewValley.Object)hoveredItem).Quality > 0)
                {
                    int quality = ((StardewValley.Object)hoveredItem).Quality;
                    if (quality == 1)
                    {
                        toSpeak = $"{toSpeak} Silver quality";
                    }
                    else if (quality == 2 || quality == 3)
                    {
                        toSpeak = $"{toSpeak} Gold quality";
                    }
                    else if (quality >= 4)
                    {
                        toSpeak = $"{toSpeak} Iridium quality";
                    }
                }
                #endregion

                #region Narrate hovered required ingredients
                if (extraItemToShowIndex != -1)
                {
                    string itemName = Game1.objectInformation[extraItemToShowIndex].Split('/')[0];

                    if (extraItemToShowAmount != -1)
                        toSpeak = $"{toSpeak} Required: {extraItemToShowAmount} {itemName}";
                    else
                        toSpeak = $"{toSpeak} Required: {itemName}";
                }
                #endregion

                #region Add money
                if (moneyAmountToDisplayAtBottom != -1)
                    toSpeak = $"{toSpeak} \nCost: {moneyAmountToDisplayAtBottom}g\n";
                #endregion

                #region Add the base text
                if (text == "???")
                    toSpeak = "unknown";
                else
                    toSpeak = $"{toSpeak} {text}";
                #endregion

                #region Add crafting ingredients
                if (craftingIngredients != null)
                {
                    toSpeak = $"{toSpeak} \n{craftingIngredients.description}";
                    toSpeak = $"{toSpeak} \nIngredients\n";

                    craftingIngredients.recipeList.ToList().ForEach(recipe =>
                    {
                        int count = recipe.Value;
                        int item = recipe.Key;
                        string name = craftingIngredients.getNameFromIndex(item);

                        toSpeak = $"{toSpeak} ,{count} {name}";
                    });
                }
                #endregion

                #region Add health & stamina
                if (hoveredItem is StardewValley.Object && ((StardewValley.Object)hoveredItem).Edibility != -300)
                {
                    int stamina_recovery = ((StardewValley.Object)hoveredItem).staminaRecoveredOnConsumption();
                    toSpeak = $"{toSpeak} {stamina_recovery} Energy\n";
                    if (stamina_recovery >= 0)
                    {
                        int health_recovery = ((StardewValley.Object)hoveredItem).healthRecoveredOnConsumption();
                        toSpeak = $"{toSpeak} {health_recovery} Health";
                    }
                }
                #endregion

                #region Add buff items (effects like +1 walking speed)
                if (buffIconsToDisplay != null)
                {
                    for (int i = 0; i < buffIconsToDisplay.Length; i++)
                    {
                        string buffName = ((Convert.ToInt32(buffIconsToDisplay[i]) > 0) ? "+" : "") + buffIconsToDisplay[i] + " ";
                        if (i <= 11)
                        {
                            buffName = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + i, buffName);
                        }
                        try
                        {
                            int count = int.Parse(buffName[..buffName.IndexOf(' ')]);
                            if (count != 0)
                                toSpeak = $"{toSpeak} {buffName}\n";
                        }
                        catch (Exception) { }
                    }
                }
                #endregion

                #region Narrate toSpeak
                // To prevent it from getting conflicted by two hover texts at the same time, two seperate methods are used.
                // For example, sometimes `Welcome to Pierre's` and the items in seeds shop get conflicted causing it to speak infinitely.

                if (toSpeak.ToString() != " ")
                {
                    if (StardewModdingAPI.Context.IsPlayerFree)
                        MainClass.ScreenReader.SayWithChecker(toSpeak.ToString(), true); // Normal Checker
                    else
                        MainClass.ScreenReader.SayWithMenuChecker(toSpeak.ToString(), true); // Menu Checker
                }
                #endregion
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate dialog:\n{e.StackTrace}\n{e.Message}");
            }
        }

        internal static void ExitThisMenuPatch(IClickableMenu __instance)
        {
            try
            {
                MainClass.DebugLog($"Closed {__instance.GetType().ToString()} menu, performing cleanup...");
                Cleanup(__instance);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void Cleanup(IClickableMenu menu)
        {
            switch (menu)
            {
                case TitleMenu:
                    TitleMenuPatch.Cleanup();
                    break;
                case CoopMenu:
                    CoopMenuPatch.Cleanup();
                    break;
                case LoadGameMenu:
                    LoadGameMenuPatch.Cleanup();
                    break;
                case AdvancedGameOptions:
                    AdvancedGameOptionsPatch.Cleanup();
                    break;
                case LetterViewerMenu:
                    LetterViwerMenuPatch.Cleanup();
                    break;
                case LevelUpMenu:
                    LevelUpMenuPatch.Cleanup();
                    break;
                case Billboard:
                    BillboardPatch.Cleanup();
                    break;
                case GameMenu:
                    GameMenuPatch.Cleanup();
                    ExitPagePatch.Cleanup();
                    OptionsPagePatch.Cleanup();
                    SocialPagePatch.Cleanup();
                    InventoryPagePatch.Cleanup();
                    CraftingPagePatch.Cleanup();
                    break;
                case JunimoNoteMenu:
                    JunimoNoteMenuPatch.Cleanup();
                    break;
                case ShopMenu:
                    ShopMenuPatch.Cleanup();
                    break;
                case ItemGrabMenu:
                    ItemGrabMenuPatch.Cleanup();
                    break;
                case GeodeMenu:
                    GeodeMenuPatch.Cleanup();
                    break;
                case CarpenterMenu:
                    CarpenterMenuPatch.Cleanup();
                    break;
                case PurchaseAnimalsMenu:
                    PurchaseAnimalsMenuPatch.Cleanup();
                    break;
                case AnimalQueryMenu:
                    AnimalQueryMenuPatch.Cleanup();
                    break;
                case DialogueBox:
                    DialogueBoxPatch.Cleanup();
                    break;
                case JojaCDMenu:
                    JojaCDMenuPatch.Cleanup();
                    break;
                case QuestLog:
                    QuestLogPatch.Cleaup();
                    break;
                case TailoringMenu:
                    TailoringMenuPatch.Cleanup();
                    break;
                case ForgeMenu:
                    ForgeMenuPatch.Cleanup();
                    break;
                case ItemListMenu:
                    ItemListMenuPatch.Cleanup();
                    break;
                case FieldOfficeMenu:
                    FieldOfficeMenuPatch.Cleanup();
                    break;
                case MuseumMenu:
                    MuseumMenuPatch.Cleanup();
                    break;
                case PondQueryMenu:
                    PondQueryMenuPatch.Cleanup();
                    break;
                case SpecialOrdersBoard:
                    SpecialOrdersBoardPatch.Cleanup();
                    break;
            }

            InventoryUtils.Cleanup();
            TextBoxPatch.activeTextBoxes = "";
        }
    }
}
