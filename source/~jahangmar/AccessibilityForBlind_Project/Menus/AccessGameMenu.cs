/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jahangmar/StardewValleyMods
**
*************************************************/

// Copyright (c) 2020 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace AccessibilityForBlind.Menus
{
    public class AccessGameMenu : AccessMenu
    {
        public AccessGameMenu(GameMenu menu) : base(menu)
        {

        }

        public override string GetTitle()
        {
            return "GameMenu";
        }

        public void SetMenuItemsForTab(bool speakPageName=true)
        {
            GameMenu menu = stardewMenu as GameMenu;
            IClickableMenu menuPage = menu.GetCurrentPage();

            ClearItems();

            if (menuPage is InventoryPage invPage)
            {
                TextToSpeech.Speak("inventory");
            }
            else if (menuPage is SkillsPage skillsPage)
            {
                TextToSpeech.Speak("skills");
                foreach (ClickableComponent comp in skillsPage.skillAreas)
                {
                    int idx = System.Convert.ToInt32(comp.name);
                    string label = Farmer.getSkillDisplayNameFromIndex(idx);
                    label += " level " + Game1.player.GetSkillLevel(idx);
                    AddItem(MenuItem.MenuItemFromComponent(comp, skillsPage, label));
                }
            }
            else if (menuPage is SocialPage socialPage)
            {
                TextToSpeech.Speak("social");
                //socialPage.setCurrentlySnappedComponentTo(menu.tabs[GameMenu.socialTab].myID);
                socialPage.setCurrentlySnappedComponentTo(socialPage.characterSlots[0].myID);
                int i = 0;
                foreach (ClickableComponent comp in socialPage.characterSlots)
                {
                    AddItem(MenuItem.MenuItemFromComponent(comp, socialPage, socialPage.names[i].ToString()));
                    i += 1;
                }
                NextItem();
            }
            else if (menuPage is MapPage mapPage)
            {
                TextToSpeech.Speak("map");
            }
            else if (menuPage is CraftingPage craftingPage)
            {
                if (speakPageName)
                    TextToSpeech.Speak("crafting");

                int currentCraftingPage = ModEntry.GetHelper().Reflection.GetField<int>(craftingPage, "currentCraftingPage").GetValue();
                Dictionary<ClickableTextureComponent, CraftingRecipe> pagesOfCraftingRecipes = ModEntry.GetHelper().Reflection.GetField<List<Dictionary<ClickableTextureComponent, CraftingRecipe>>>(craftingPage, "pagesOfCraftingRecipes").GetValue()[currentCraftingPage];
                IReflectedMethod getContainerContents = ModEntry.GetHelper().Reflection.GetMethod(craftingPage, "getContainerContents");
                int i = 0;
                foreach (ClickableTextureComponent comp in craftingPage.currentPageClickableComponents)
                {
                    CraftingRecipe recipe = pagesOfCraftingRecipes[comp];
                    bool doesFarmerHasItems = recipe.doesFarmerHaveIngredientsInInventory(getContainerContents.Invoke<IList<Item>>());
                    Dictionary<int, int> recipeList = ModEntry.GetHelper().Reflection.GetField<Dictionary<int, int>>(recipe, "recipeList").GetValue();
                    string description = ModEntry.GetHelper().Reflection.GetField<string>(recipe, "description").GetValue();

                    string recipeListToDescr()
                    {
                        string result = "";
                        foreach (KeyValuePair<int,int> ingred in recipeList)
                        {
                            result += ingred.Value + " " + Game1.objectInformation[ingred.Key].Split('/')[4] + ",";
                        }
                        return result;
                    }

                    MenuItem menuItem = MenuItem.MenuItemFromComponent(comp, craftingPage, recipe.DisplayName + (doesFarmerHasItems ? "" : " missing resources"));
                    //string craftable = doesFarmerHasItems ? "can be crafted," : "resources missing";
                    menuItem.Description = " needs " + recipeListToDescr() + " " + description;
                    AddItem(menuItem);
                    i++;
                }
            }
            else if (menuPage is CollectionsPage collectionsPage)
            {
                TextToSpeech.Speak("collections");
            }
            else if (menuPage is OptionsPage optionsPage)
            {
                TextToSpeech.Speak("options");
            }
            else if (menuPage is ExitPage exitPage)
            {
                TextToSpeech.Speak("exit");
                MenuItem exitToTitle = MenuItem.MenuItemFromComponent(exitPage.exitToTitle, menu, exitPage.exitToTitle.label);
                exitToTitle.TextOnAction = "exiting to title";
                AddItem(exitToTitle);
                MenuItem exitToDesktop = MenuItem.MenuItemFromComponent(exitPage.exitToDesktop, menu, exitPage.exitToDesktop.label);
                exitToDesktop.TextOnAction = "closing game";
                AddItem(exitToDesktop);
            }
            else
            {
                TextToSpeech.Speak("unsupported");
            }
        }

        public override void ButtonPressed(SButton button)
        {
            GameMenu menu = stardewMenu as GameMenu;

            if (Inputs.IsGameMenuInventoryButton(button))
            {
                menu.changeTab(GameMenu.inventoryTab);
                SetMenuItemsForTab();
            }
            else if (Inputs.IsGameMenuSkillsButton(button))
            {
                menu.changeTab(GameMenu.skillsTab);
                SetMenuItemsForTab();
            }
            else if (Inputs.IsGameMenuSocialButton(button))
            {
                menu.changeTab(GameMenu.socialTab);
                SetMenuItemsForTab();
            }
            else if (Inputs.IsGameMenuMapButton(button))
            {
                menu.changeTab(GameMenu.mapTab);
                SetMenuItemsForTab();
            }
            else if (Inputs.IsGameMenuCraftingButton(button))
            {
                menu.changeTab(GameMenu.craftingTab);
                SetMenuItemsForTab();
            }
            else if (Inputs.IsGameMenuCollectionsButton(button))
            {
                menu.changeTab(GameMenu.collectionsTab);
                SetMenuItemsForTab();
            }
            else if (Inputs.IsGameMenuOptionsButton(button))
            {
                menu.changeTab(GameMenu.optionsTab);
                SetMenuItemsForTab();
            }
            else if (Inputs.IsGameMenuExitButton(button))
            {
                menu.changeTab(GameMenu.exitTab);
                SetMenuItemsForTab();
            }
            else if (Inputs.IsMenuEscapeButton(button))
            {
                menu.exitThisMenu();
                ModEntry.GetHelper().Input.Suppress(button);
            }
            else if (menu.GetCurrentPage() is SocialPage socialPage && (Inputs.IsMenuNextButton(button) || Inputs.IsMenuPrevButton(button)))
            {
                base.ButtonPressed(button);
                if (Inputs.IsMenuNextButton(button))
                {
                    ClickableComponent oldComp = socialPage.getCurrentlySnappedComponent();
                    socialPage.applyMovementKey(2);
                    if (oldComp == socialPage.getCurrentlySnappedComponent())
                    {
                        socialPage.setCurrentlySnappedComponentTo(socialPage.characterSlots[0].myID);
                        ModEntry.GetHelper().Reflection.GetMethod(socialPage, "_SelectSlot").Invoke(socialPage.getCurrentlySnappedComponent());
                    }
                }
                else if (Inputs.IsMenuPrevButton(button))
                {
                    //ClickableComponent oldComp = socialPage.getCurrentlySnappedComponent();
                    socialPage.applyMovementKey(0);
                    if (menu.tabs[GameMenu.socialTab] == menu.getCurrentlySnappedComponent())
                    {
                        socialPage.setCurrentlySnappedComponentTo(socialPage.characterSlots[socialPage.characterSlots.Count - 1].myID);
                        ModEntry.GetHelper().Reflection.GetMethod(socialPage, "_SelectSlot").Invoke(socialPage.getCurrentlySnappedComponent());
                    }
                }
            }
            else if (menu.GetCurrentPage() is CraftingPage craftingPage && (Inputs.IsMenuNextButton(button) || Inputs.IsMenuPrevButton(button)))
            {
                int craftingPagesCount = ModEntry.GetHelper().Reflection.GetField<List<Dictionary<ClickableTextureComponent, CraftingRecipe>>>(craftingPage, "pagesOfCraftingRecipes").GetValue().Count;
                int currentCraftingPage = ModEntry.GetHelper().Reflection.GetField<int>(craftingPage, "currentCraftingPage").GetValue();
                if (Inputs.IsMenuNextButton(button) && IsOnLastItem())
                {
                    if (currentCraftingPage >= craftingPagesCount - 1) //on last page
                    {
                        ModEntry.GetHelper().Reflection.GetField<int>(craftingPage, "currentCraftingPage").SetValue(0);

                    }
                    else //not on last page
                    {
                        ModEntry.GetHelper().Reflection.GetField<int>(craftingPage, "currentCraftingPage").SetValue(currentCraftingPage + 1);
                    }
                    ModEntry.GetHelper().Reflection.GetMethod(craftingPage, "_UpdateCurrentPageButtons").Invoke();
                    SetMenuItemsForTab(speakPageName: false);
                    NextItem();
                }
                else if (Inputs.IsMenuPrevButton(button) && IsOnFirstItem())
                {
                    if (currentCraftingPage == 0) //on first page
                    {
                        ModEntry.GetHelper().Reflection.GetField<int>(craftingPage, "currentCraftingPage").SetValue(craftingPagesCount - 1);
                    }
                    else // not on first page
                    {
                        ModEntry.GetHelper().Reflection.GetField<int>(craftingPage, "currentCraftingPage").SetValue(currentCraftingPage - 1);
                    }
                    ModEntry.GetHelper().Reflection.GetMethod(craftingPage, "_UpdateCurrentPageButtons").Invoke();
                    SetMenuItemsForTab(speakPageName: false);
                    PrevItem();
                    PrevItem();
                }
                else
                    base.ButtonPressed(button);
            }
            else
                base.ButtonPressed(button);
        }
    }
}
