using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace RecipeOrganizer
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        ClickableTextureComponent organizeButton;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            ControlEvents.MouseChanged += ControlEvents_MouseChanged;
            GraphicsEvents.Resize += GraphicsEvents_Resize;
            GraphicsEvents.OnPreRenderGuiEvent += GraphicsEvents_OnPreRenderGuiEvent;
        }
        /*********
        ** Private methods
        *********/
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// 
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            InitializeButton();
        }

        private void ControlEvents_MouseChanged(object sender, EventArgsMouseStateChanged e)
        {
            if (isInRecipeMenu())
            {
                if (e.NewState.LeftButton == ButtonState.Pressed && e.PriorState.LeftButton == ButtonState.Released)
                {
                    if (organizeButton.bounds.Contains(e.NewPosition.X, e.NewPosition.Y))
                    {
                        this.Monitor.Log("I'm clicking");
                        SortRecipes();
                    }
                }
            }
        }

        private void SortRecipes()
        {

            List<Dictionary<ClickableTextureComponent, CraftingRecipe>> pagesOfRecipes = ((CraftingPage)Game1.activeClickableMenu).pagesOfCraftingRecipes;
            Dictionary<string, int> farmerRecipes = Game1.player.cookingRecipes;

            //I may be able to use currentRecipePage to play with x and y depending on some things
            //int currentRecipePage = this.Helper.Reflection.GetPrivateValue<int>(((CraftingPage)Game1.activeClickableMenu), "currentCraftingPage"); 

            Dictionary<ClickableTextureComponent, int> knownRecipes = new Dictionary<ClickableTextureComponent, int>();
            Dictionary<ClickableTextureComponent, int> unknownRecipes = new Dictionary<ClickableTextureComponent, int>();
            //List<Vector2[]> recipesLocationsPages = new List<Vector2[]>();

            //Getting and saying recipe textures (to change x and y) and then all recipe locations on each page
            //Well, I just noticed that page 1 and page 2 are independent... so I can't just change x, y I need a way to put stuff from p2 on page 1
            for (int i = 0; i < pagesOfRecipes.Count; i++)
            {
                //List<Vector2> recipesLocations = new List<Vector2>();

                foreach (KeyValuePair<ClickableTextureComponent, CraftingRecipe> page in pagesOfRecipes[i])
                {
                    //recipesLocations.Add(new Vector2(page.Key.bounds.X, page.Key.bounds.Y));
                    bool unknown = true;

                    foreach (string recipeName in farmerRecipes.Keys)
                    {
                        if (recipeName == page.Value.DisplayName)
                        {
                            knownRecipes.Add(page.Key,i);
                            unknown = false;
                        }
                    }

                    if (unknown)
                        unknownRecipes.Add(page.Key,i);
                }

                //recipesLocationsPages.Add(recipesLocations.ToArray());
            }

            //Was thinking of changing the collection directly, but turns out that it stops looping if a collection is modified
            //foreach(KeyValuePair<ClickableTextureComponent, int> knownRecipe in knownRecipes)
            //{
            //    foreach(KeyValuePair<ClickableTextureComponent, int> unknownRecipe in unknownRecipes)
            //    {
            //        //if an unknown recipe is on page 1 while known is on page 2
            //        if (knownRecipe.Value == 1 && unknownRecipe.Value == 0)
            //        {
            //            foreach (KeyValuePair<ClickableTextureComponent, CraftingRecipe> mainRecipesPage in pagesOfRecipes[0])
            //            {
            //                if (mainRecipesPage.Key == unknownRecipe.Key)
            //                {
            //                    foreach (KeyValuePair<ClickableTextureComponent, CraftingRecipe> mainRecipesPage2 in pagesOfRecipes[1])
            //                    {
            //                        if (mainRecipesPage2.Key == knownRecipe.Key)
            //                        {
            //                            //removes the unknown from page 1 and known from page 2
            //                            //add known to page 1 and add unknown to page 2
            //                            pagesOfRecipes[0].Remove(mainRecipesPage.Key);
            //                            pagesOfRecipes[1].Remove(mainRecipesPage2.Key);
            //                            pagesOfRecipes[0].Add(mainRecipesPage2.Key, mainRecipesPage2.Value);
            //                            pagesOfRecipes[1].Add(mainRecipesPage.Key, mainRecipesPage.Value);
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}


            //Sort the recipe locations for known recipes
            //foreach(Vector2[] pageLocations in recipesLocationsPages)
            //{
            //    for (int i = 1; i < pageLocations.Length; i++)
            //    {
            //        while (pageLocations[i].X < pageLocations[i - 1].X && pageLocations[i].Y < pageLocations[i - 1].Y)
            //        {
            //            Vector2 temp = pageLocations[i - 1];
            //            pageLocations[i - 1] = pageLocations[i];
            //            pageLocations[i] = temp;
            //        }
            //    }
            //}

            //ClickableTextureComponent[] knownRecipesArray = knownRecipes.ToArray();
            //ClickableTextureComponent[] unknownRecipesArray = unknownRecipes.ToArray();

            //for (int index=0; index < knownRecipesArray.Length; index++)
            //{
            //    knownRecipesArray[index].bounds.X = (int)recipesLocationsPages[0][index].X;
            //    knownRecipesArray[index].bounds.Y = (int)recipesLocationsPages[0][index].Y;
            //}

            //for(int index=knownRecipesArray.Length; index<recipesLocationsPages[0].Length;index++)
            //{
            //    unknownRecipesArray[index].bounds.X = (int)recipesLocationsPages[0][index].X;
            //    unknownRecipesArray[index].bounds.Y = (int)recipesLocationsPages[0][index].Y;
            //}
        }

        private void InitializeButton()
        {
            int x = Game1.viewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2;
            int y = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2;
            int width = 800 + IClickableMenu.borderWidth * 2;
            int height = 600 + IClickableMenu.borderWidth * 2;

            Rectangle bounds = new Rectangle(x + width, y + height / 3 - Game1.tileSize, Game1.tileSize, Game1.tileSize);
            Rectangle sourceRect = new Rectangle(162, 440, 16, 16);

            organizeButton = new ClickableTextureComponent("", bounds, "", Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"), Game1.mouseCursors, sourceRect, (float)Game1.pixelZoom, false);
        }

        //Won't do anything until a new SMAPI update (which should fix it)
        //https://github.com/Pathoschild/SMAPI/issues/328
        private void GraphicsEvents_Resize(object sender, EventArgs e)
        {
            InitializeButton();
        }

        private void GraphicsEvents_OnPreRenderGuiEvent(object sender, EventArgs e)
        {
            if(isInRecipeMenu())
                organizeButton.draw(Game1.spriteBatch);
        }

        private bool isInRecipeMenu()
        {
            IClickableMenu menu = Game1.activeClickableMenu;
            if (Game1.activeClickableMenu != null && menu is CraftingPage)
            {
                //if it is the recipe menu
                return this.Helper.Reflection.GetField<bool>(menu, "cooking").GetValue();
            }
            return false;
        }
    }
}