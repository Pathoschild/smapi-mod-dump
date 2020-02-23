using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;

namespace SB_Builderslist
{
    internal class ModEntry : Mod
    {
        ScavengerMenu scavengermenu;
        private bool isReady, isHidden;
        private bool checkForClick;
        private bool scanForCraftingPageOnGameMenu;
        CraftingPage currentCraftingPage;
        ModConfig config;


        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Display.RenderedHud += OnrenderedHUD;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnToTitle;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            currentCraftingPage = null;
            checkForClick = false;
            scanForCraftingPageOnGameMenu = false;
            isReady = true;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (isHidden) return;
            if (!scanForCraftingPageOnGameMenu) return;
            //check if we are still in the GameMenu. if not, stop updating
            if (!(Game1.activeClickableMenu is GameMenu))
            {
                scanForCraftingPageOnGameMenu = false;
                return;
            }
            //check if we are in the Crafting Page
            if ((Game1.activeClickableMenu as GameMenu).currentTab != GameMenu.craftingTab) return;

            currentCraftingPage = (CraftingPage)(Game1.activeClickableMenu as GameMenu).GetCurrentPage();
            scavengermenu.iscooking = this.Helper.Reflection.GetField<bool>(currentCraftingPage, "cooking").GetValue();
            checkForClick = true;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!isReady) return;
            if (!isHidden && checkForClick)
            {
                if (e.Button.Equals(SButton.MouseLeft) && Game1.oldKBState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                {
                    List<Dictionary<ClickableTextureComponent, CraftingRecipe>> recipes = this.Helper.Reflection.GetField<List<Dictionary<ClickableTextureComponent, CraftingRecipe>>>(currentCraftingPage, "pagesOfCraftingRecipes").GetValue();

                    int currentCraftingPagePage = this.Helper.Reflection.GetField<int>(currentCraftingPage, "currentCraftingPage").GetValue();
                    foreach (ClickableTextureComponent button in recipes[currentCraftingPagePage].Keys)
                    {
                        if (button.containsPoint((int)e.Cursor.ScreenPixels.X, (int)e.Cursor.ScreenPixels.Y))
                        {
                            scavengermenu.ScavengerRecipe = recipes[currentCraftingPagePage][button];
                            scavengermenu.recipeListNeedsUpdate = true;
                            this.config.currentRecipe = scavengermenu.ScavengerRecipe.name;
                            this.config.isCooking = scavengermenu.ScavengerRecipe.isCookingRecipe;
                            this.Helper.WriteConfig<ModConfig>(this.config);
                        }
                    }
                    this.Helper.Input.Suppress(e.Button);
                }
            }
            if(e.Button.Equals(SButton.E) && Game1.oldKBState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
            {
                isHidden = !isHidden;
                if (!isHidden)
                {
                    Game1.onScreenMenus.Add(scavengermenu);
                }
                else
                {
                    Game1.onScreenMenus.Remove(scavengermenu); 
                }
                this.Helper.Input.Suppress(e.Button);
            }
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (isHidden) return;
            if (e.NewMenu is GameMenu menu)
            {
                if (menu.currentTab == GameMenu.craftingTab)
                {
                    currentCraftingPage = (CraftingPage)(Game1.activeClickableMenu as GameMenu).GetCurrentPage();
                    scavengermenu.iscooking = this.Helper.Reflection.GetField<bool>(currentCraftingPage, "cooking").GetValue();
                    checkForClick = true;
                    return;
                }
                // I suspect that unless it immediately goes to the Crafting Tab, it's not going to update, so I need to check every tick for page changes
                scanForCraftingPageOnGameMenu = true;
                return;
            }
            if (e.NewMenu is CraftingPage)
            {
                currentCraftingPage = (e.NewMenu as CraftingPage);
                scavengermenu.iscooking = this.Helper.Reflection.GetField<bool>(currentCraftingPage, "cooking").GetValue();
                checkForClick = true;
                return;
            }
            checkForClick = false;
            scanForCraftingPageOnGameMenu = false;
        }

        private void OnReturnToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            isReady = false;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            scavengermenu = new ScavengerMenu(null, this.Helper.Reflection);
            this.config = this.Helper.ReadConfig<ModConfig>();
            isHidden = !this.config.isActive;
            if (this.config.currentRecipe != null)
            { 
                scavengermenu.ScavengerRecipe = new CraftingRecipe(this.config.currentRecipe, this.config.isCooking);
                scavengermenu.recipeListNeedsUpdate = true;
            }

            if (!isHidden) Game1.onScreenMenus.Add(scavengermenu);
            isReady = true;
        }

        private void OnrenderedHUD(object sender, RenderedHudEventArgs e)
        {
            if (!isReady)
                return;
        }
    }
}