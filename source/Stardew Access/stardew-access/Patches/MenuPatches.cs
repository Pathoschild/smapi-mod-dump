/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using stardew_access.Features;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class MenuPatches
    {
        private static string currentLetterText = " ";
        private static string museumQueryKey = " ";
        private static string currentLevelUpTitle = " ";
        public static Vector2? prevTile = null;
        private static bool isMoving = false;

        #region Museum Menu Patch
        internal static bool MuseumMenuKeyPressPatch()
        {
            try
            {
                if (isMoving)
                    return false;

                if (!isMoving)
                {
                    isMoving = true;
                    Task.Delay(200).ContinueWith(_ => { isMoving = false; });
                }

            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }

            return true;
        }

        internal static void MuseumMenuPatch(MuseumMenu __instance, bool ___holdingMuseumPiece)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (__instance.heldItem != null)
                {
                    // Museum Inventory
                    string toSpeak = "";
                    int tileX = (int)(Utility.ModifyCoordinateFromUIScale(x) + (float)Game1.viewport.X) / 64;
                    int tileY = (int)(Utility.ModifyCoordinateFromUIScale(y) + (float)Game1.viewport.Y) / 64;
                    LibraryMuseum libraryMuseum = (LibraryMuseum)Game1.currentLocation;

                    if (libraryMuseum.isTileSuitableForMuseumPiece(tileX, tileY))
                        toSpeak = $"slot {tileX}x {tileY}y";

                    if (museumQueryKey != toSpeak)
                    {
                        museumQueryKey = toSpeak;
                        MainClass.GetScreenReader().Say(toSpeak, true);
                    }
                }
                else
                {
                    // Player Inventory
                    if (!narrateHoveredItemInInventory(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y))
                    {
                        if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                        {
                            if (museumQueryKey != $"ok button")
                            {
                                museumQueryKey = $"ok button";
                                MainClass.GetScreenReader().Say("ok button", true);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
        #endregion

        internal static bool narrateHoveredItemInInventory(InventoryMenu inventoryMenu, List<ClickableComponent> inventory, IList<Item> actualInventory, int x, int y)
        {
            #region Narrate hovered item
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].containsPoint(x, y))
                {
                    string toSpeak = "";
                    if ((i + 1) <= actualInventory.Count)
                    {
                        if (actualInventory[i] != null)
                        {
                            string name = actualInventory[i].DisplayName;
                            int stack = actualInventory[i].Stack;
                            string quality = "";

                            #region Add quality of item
                            if (actualInventory[i] is StardewValley.Object && ((StardewValley.Object)actualInventory[i]).quality > 0)
                            {
                                int qualityIndex = ((StardewValley.Object)actualInventory[i]).quality;
                                if (qualityIndex == 1)
                                {
                                    quality = "Silver quality";
                                }
                                else if (qualityIndex == 2 || qualityIndex == 3)
                                {
                                    quality = "Gold quality";
                                }
                                else if (qualityIndex >= 4)
                                {
                                    quality = "Iridium quality";
                                }
                            }
                            #endregion

                            if (inventoryMenu.highlightMethod(inventoryMenu.actualInventory[i]))
                                name = $"Donatable {name}";

                            if (stack > 1)
                                toSpeak = $"{stack} {name} {quality}";
                            else
                                toSpeak = $"{name} {quality}";
                        }
                        else
                        {
                            // For empty slot
                            toSpeak = "Empty Slot";
                        }
                    }
                    else
                    {
                        // For empty slot
                        toSpeak = "Empty Slot";
                    }

                    if (museumQueryKey != $"{toSpeak}:{i}")
                    {
                        museumQueryKey = $"{toSpeak}:{i}";
                        MainClass.GetScreenReader().Say(toSpeak, true);
                    }
                    return true;
                }
            }
            #endregion
            return false;
        }

        internal static bool PlaySoundPatch(string cueName)
        {
            try
            {
                if (!Context.IsPlayerFree)
                    return true;

                if (!Game1.player.isMoving())
                    return true;

                if (cueName == "grassyStep" || cueName == "sandyStep" || cueName == "snowyStep" || cueName == "stoneStep" || cueName == "thudStep" || cueName == "woodyStep")
                {
                    Vector2 nextTile = CurrentPlayer.getNextTile();
                    if (ReadTile.isCollidingAtTile((int)nextTile.X, (int)nextTile.Y))
                    {
                        if (prevTile != nextTile)
                        {
                            prevTile = nextTile;
                            //Game1.playSound("colliding");
                        }
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }

            return true;
        }

        internal static void LanguageSelectionMenuPatch(LanguageSelectionMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (__instance.nextPageButton != null && __instance.nextPageButton.containsPoint(x, y))
                {
                    MainClass.GetScreenReader().SayWithMenuChecker($"Next Page Button", true);
                    return;
                }

                if (__instance.previousPageButton != null && __instance.previousPageButton.containsPoint(x, y))
                {
                    MainClass.GetScreenReader().SayWithMenuChecker($"Previous Page Button", true);
                    return;
                }

                for (int i = 0; i < __instance.languages.Count; i++)
                {
                    if (__instance.languages[i].containsPoint(x, y))
                    {
                        MainClass.GetScreenReader().SayWithMenuChecker($"{__instance.languageList[i]} Button", true);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void MineElevatorMenuPatch(List<ClickableComponent> ___elevators)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                for (int i = 0; i < ___elevators.Count; i++)
                {
                    if (___elevators[i].containsPoint(x, y))
                    {
                        MainClass.GetScreenReader().SayWithMenuChecker($"{___elevators[i].name} level", true);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void NamingMenuPatch(NamingMenu __instance, string title, TextBox ___textBox)
        {
            try
            {
                __instance.textBoxCC.snapMouseCursor();
                ___textBox.SelectMe();
                string toSpeak = $"{title}";

                MainClass.GetScreenReader().SayWithChecker(toSpeak, true);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void ConfirmationDialogPatch(ConfirmationDialog __instance, string ___message)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);

                MainClass.GetScreenReader().SayWithMenuChecker(___message, true);
                if (__instance.okButton.containsPoint(x, y))
                {
                    MainClass.GetScreenReader().SayWithMenuChecker("Ok Button", false);
                }
                else if (__instance.cancelButton.containsPoint(x, y))
                {
                    MainClass.GetScreenReader().SayWithMenuChecker("Cancel Button", false);
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void LevelUpMenuPatch(LevelUpMenu __instance, List<int> ___professionsToChoose, List<string> ___leftProfessionDescription, List<string> ___rightProfessionDescription, List<string> ___extraInfoForLevel, List<CraftingRecipe> ___newCraftingRecipes, string ___title, bool ___isActive, bool ___isProfessionChooser)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
                string leftProfession = " ", rightProfession = " ", extraInfo = " ", newCraftingRecipe = " ", toSpeak = " ";

                if (!__instance.informationUp)
                {
                    return;
                }
                if (__instance.isProfessionChooser)
                {
                    if (___professionsToChoose.Count() == 0)
                    {
                        return;
                    }
                    for (int j = 0; j < ___leftProfessionDescription.Count; j++)
                    {
                        leftProfession += ___leftProfessionDescription[j] + ", ";
                    }
                    for (int i = 0; i < ___rightProfessionDescription.Count; i++)
                    {
                        rightProfession += ___rightProfessionDescription[i] + ", ";
                    }

                    if (__instance.leftProfession.containsPoint(x, y))
                    {
                        if ((MainClass.Config.LeftClickMainKey.JustPressed() || MainClass.Config.LeftClickAlternateKey.JustPressed()) && __instance.readyToClose())
                        {
                            Game1.player.professions.Add(___professionsToChoose[0]);
                            __instance.getImmediateProfessionPerk(___professionsToChoose[0]);
                            ___isActive = false;
                            __instance.informationUp = false;
                            ___isProfessionChooser = false;
                            __instance.RemoveLevelFromLevelList();
                            __instance.exitThisMenu();
                            return;
                        }

                        toSpeak = $"Selected: {leftProfession} Left click to choose.";
                    }

                    if (__instance.rightProfession.containsPoint(x, y))
                    {
                        if ((MainClass.Config.LeftClickMainKey.JustPressed() || MainClass.Config.LeftClickAlternateKey.JustPressed()) && __instance.readyToClose())
                        {
                            Game1.player.professions.Add(___professionsToChoose[1]);
                            __instance.getImmediateProfessionPerk(___professionsToChoose[1]);
                            ___isActive = false;
                            __instance.informationUp = false;
                            ___isProfessionChooser = false;
                            __instance.RemoveLevelFromLevelList();
                            __instance.exitThisMenu();
                            return;
                        }

                        toSpeak = $"Selected: {rightProfession} Left click to choose.";
                    }
                }
                else
                {
                    foreach (string s2 in ___extraInfoForLevel)
                    {
                        extraInfo += s2 + ", ";
                    }
                    foreach (CraftingRecipe s in ___newCraftingRecipes)
                    {
                        string cookingOrCrafting = Game1.content.LoadString("Strings\\UI:LearnedRecipe_" + (s.isCookingRecipe ? "cooking" : "crafting"));
                        string message = Game1.content.LoadString("Strings\\UI:LevelUp_NewRecipe", cookingOrCrafting, s.DisplayName);

                        newCraftingRecipe += $"{message}, ";
                    }
                }

                if (__instance.okButton.containsPoint(x, y))
                {
                    if (MainClass.Config.LeftClickMainKey.JustPressed() || MainClass.Config.LeftClickAlternateKey.JustPressed())
                        __instance.okButtonClicked();

                    toSpeak = $"{___title} {extraInfo} {newCraftingRecipe}. Left click to close.";
                }

                if (toSpeak != " ")
                    MainClass.GetScreenReader().SayWithMenuChecker(toSpeak, true);
                else if (__instance.isProfessionChooser && currentLevelUpTitle != $"{___title}. Select a new profession.")
                {
                    MainClass.GetScreenReader().SayWithMenuChecker($"{___title}. Select a new profession.", true);
                    currentLevelUpTitle = $"{___title}. Select a new profession.";
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void ShippingMenuPatch(ShippingMenu __instance, List<int> ___categoryTotals)
        {
            try
            {

                if (__instance.currentPage == -1)
                {
                    int total = ___categoryTotals[5];
                    string toSpeak;
                    if (__instance.okButton.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                    {
                        // Perform Left Click
                        if (MainClass.Config.LeftClickMainKey.JustPressed() || MainClass.Config.LeftClickAlternateKey.JustPressed())
                        {
                            Game1.activeClickableMenu.receiveLeftClick(Game1.getMouseX(true), Game1.getMouseY(true));
                        }
                        toSpeak = $"{total}g in total. Press left mouse button to save.";
                        MainClass.GetScreenReader().SayWithChecker(toSpeak, true);
                    }
                    for (int i = 0; i < __instance.categories.Count; i++)
                    {
                        if (__instance.categories[i].containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                        {
                            toSpeak = $"Money recieved from {__instance.getCategoryName(i)}: {___categoryTotals[i]}g.";
                            MainClass.GetScreenReader().SayWithChecker(toSpeak, true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void LetterViewerMenuPatch(LetterViewerMenu __instance)
        {
            try
            {
                if (!__instance.IsActive())
                    return;

                int x = Game1.getMousePosition().X, y = Game1.getMousePosition().Y;
                #region Texts in the letter
                string message = __instance.mailMessage[__instance.page];

                string toSpeak = $"{message}";

                if (__instance.ShouldShowInteractable())
                {
                    if (__instance.moneyIncluded > 0)
                    {
                        string moneyText = Game1.content.LoadString("Strings\\UI:LetterViewer_MoneyIncluded", __instance.moneyIncluded);
                        toSpeak += $"\t\n\t ,Included money: {moneyText}";
                    }
                    else if (__instance.learnedRecipe != null && __instance.learnedRecipe.Length > 0)
                    {
                        string recipeText = Game1.content.LoadString("Strings\\UI:LetterViewer_LearnedRecipe", __instance.cookingOrCrafting);
                        toSpeak += $"\t\n\t ,Learned Recipe: {recipeText}";
                    }
                }

                if (currentLetterText != toSpeak)
                {
                    currentLetterText = toSpeak;

                    // snap mouse to accept quest button
                    if (__instance.acceptQuestButton != null && __instance.questID != -1)
                    {
                        toSpeak += "\t\n Left click to accept quest.";
                        __instance.acceptQuestButton.snapMouseCursorToCenter();
                    }
                    if (__instance.mailMessage.Count > 1)
                        toSpeak = $"Page {__instance.page + 1} of {__instance.mailMessage.Count}:\n\t{toSpeak}";

                    MainClass.GetScreenReader().Say(toSpeak, false);
                }
                #endregion

                #region Narrate items given in the mail
                if (__instance.ShouldShowInteractable())
                {
                    foreach (ClickableComponent c in __instance.itemsToGrab)
                    {
                        string name = c.name;
                        string label = c.label;

                        if (c.containsPoint(x, y))
                            MainClass.GetScreenReader().SayWithChecker($"Grab: {name} \t\n {label}", false);
                    }
                }
                #endregion

                #region Narrate buttons
                if (__instance.backButton != null && __instance.backButton.visible && __instance.backButton.containsPoint(x, y))
                    MainClass.GetScreenReader().SayWithChecker($"Previous page button", false);

                if (__instance.forwardButton != null && __instance.forwardButton.visible && __instance.forwardButton.containsPoint(x, y))
                    MainClass.GetScreenReader().SayWithChecker($"Next page button", false);

                #endregion
            }
            catch (Exception e)
            {

                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        #region Cleanup on exitting a menu
        internal static void Game1ExitActiveMenuPatch()
        {
            try
            {
                Cleanup(Game1.activeClickableMenu);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void IClickableMenuOnExitPatch(IClickableMenu __instance)
        {
            try
            {
                Cleanup(__instance);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void Cleanup(IClickableMenu menu)
        {
            if (menu is GameMenu)
            {
                GameMenuPatches.gameMenuQueryKey = "";
                GameMenuPatches.craftingPageQueryKey = "";
                GameMenuPatches.inventoryPageQueryKey = "";
                GameMenuPatches.exitPageQueryKey = "";
                GameMenuPatches.optionsPageQueryKey = "";
                GameMenuPatches.socialPageQuery = "";
                GameMenuPatches.currentSelectedCraftingRecipe = -1;
                GameMenuPatches.isSelectingRecipe = false;
            }

            if (menu is JunimoNoteMenu)
            {
                GameMenuPatches.currentIngredientListItem = -1;
                GameMenuPatches.currentIngredientInputSlot = -1;
                GameMenuPatches.currentInventorySlot = -1;
                GameMenuPatches.junimoNoteMenuQuery = "";
            }

            if (menu is ShopMenu)
            {
                GameMenuPatches.shopMenuQueryKey = "";
            }

            if (menu is ItemGrabMenu)
            {
                GameMenuPatches.itemGrabMenuQueryKey = "";
            }

            if (menu is GeodeMenu)
            {
                GameMenuPatches.geodeMenuQueryKey = "";
            }

            if (menu is CarpenterMenu)
            {
                BuildingNAnimalMenuPatches.carpenterMenuQuery = "";
                BuildingNAnimalMenuPatches.isUpgrading = false;
                BuildingNAnimalMenuPatches.isDemolishing = false;
                BuildingNAnimalMenuPatches.isPainting = false;
                BuildingNAnimalMenuPatches.isMoving = false;
                BuildingNAnimalMenuPatches.isConstructing = false;
                BuildingNAnimalMenuPatches.carpenterMenu = null;
            }

            if (menu is PurchaseAnimalsMenu)
            {
                BuildingNAnimalMenuPatches.purchaseAnimalMenuQuery = "";
                BuildingNAnimalMenuPatches.firstTimeInNamingMenu = true;
                BuildingNAnimalMenuPatches.purchaseAnimalsMenu = null;
            }

            if (menu is DialogueBox)
            {
                DialoguePatches.isDialogueAppearingFirstTime = true;
                DialoguePatches.currentDialogue = " ";
            }

            GameMenuPatches.hoveredItemQueryKey = "";
        }
        #endregion

        internal static void ExitEventPatch()
        {
            if (MainClass.GetScreenReader() != null)
                MainClass.GetScreenReader().CloseScreenReader();
        }
        internal static void resetGlobalVars()
        {
            currentLetterText = " ";
            currentLevelUpTitle = " ";
        }
    }
}
