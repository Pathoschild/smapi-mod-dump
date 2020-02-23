using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace SB_Builderslist
{
    internal class ScavengerMenu : IClickableMenu
    {
        public CraftingRecipe ScavengerRecipe;
        private Dictionary<int, int> currentRecipeList;
        public StardewModdingAPI.IReflectionHelper Reflection;
        public bool iscooking, recipeListNeedsUpdate;
        private ClickableComponent button;
        private Rectangle initalPosition;

        public ScavengerMenu(
        Item lastScavengerItem,
        StardewModdingAPI.IReflectionHelper reflection)
      : base(IClickableMenu.spaceToClearSideBorder, Game1.viewport.Height - ChatBox.chatboxHeight - IClickableMenu.spaceToClearSideBorder, Game1.tileSize, Game1.tileSize, false)
        {
            this.Reflection = reflection;
            initalPosition = new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
            if (ScavengerRecipe == null)
                button = new ClickableComponent(initalPosition, "");
            //until I can populate the recipe, button is still this
            button = new ClickableComponent(initalPosition, "");
            currentRecipeList = new Dictionary<int, int>();
        }
        private void getRecipeList(StardewModdingAPI.IReflectionHelper reflection)
        {
            this.currentRecipeList = reflection.GetField<Dictionary<int, int>>(ScavengerRecipe, "recipeList").GetValue();
            this.recipeListNeedsUpdate = !this.recipeListNeedsUpdate;
        }

        public void getDimensions()
        {
            initalPosition.X = IClickableMenu.spaceToClearSideBorder;
            initalPosition.Y = Game1.viewport.Height - ChatBox.chatboxHeight - IClickableMenu.spaceToClearSideBorder;

            if (ScavengerRecipe == null)
            {
                this.xPositionOnScreen = initalPosition.X;
                this.yPositionOnScreen = initalPosition.Y;
                this.width = initalPosition.Width;
                this.height = initalPosition.Height;
            }
            else
            {
                int craftableCount = ScavengerRecipe.getCraftableCount((IList<Item>)null);
                string text1 = " (" + (object)craftableCount + ")";
                Vector2 vector2_1 = Game1.smallFont.MeasureString(text1);

                this.width = (int)Math.Max((float)(double)vector2_1.X + 12.0, 320f);
                this.height = this.getDescriptionHeight(this.width);
                this.xPositionOnScreen = initalPosition.X;
                this.yPositionOnScreen = initalPosition.Y - this.height;
            }
        }
        //Draws a box to put the item in the scavenger menu if none is already there, else draws the recipe text.
        public override void draw(SpriteBatch b)
        {
            this.getDimensions();
            Game1.DrawBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
            if (ScavengerRecipe != null)
            {
                this.drawScavengerList(b, new Vector2(this.xPositionOnScreen, this.yPositionOnScreen - 16), this.width, (IList<Item>)null);

            }
            this.button.bounds = new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
        }

        public override void performHoverAction(int x, int y)
        {
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (button.containsPoint(x, y))
            {
                if (!iscooking && ScavengerRecipe.doesFarmerHaveIngredientsInInventory(null))
                {
                    Item obj = ScavengerRecipe.createItem();
                    ScavengerRecipe.consumeIngredients(null);
                    Game1.playSound("crafting");

                    if (!this.iscooking && Game1.player.craftingRecipes.ContainsKey(ScavengerRecipe.name))
                    {
                        Game1.player.craftingRecipes[ScavengerRecipe.name] += ScavengerRecipe.numberProducedPerCraft;
                    }
                    if (this.iscooking)
                    {
                        Game1.player.cookedRecipe(obj.ParentSheetIndex);
                    }
                    if (!this.iscooking)
                    {
                        Game1.stats.checkForCraftingAchievements();
                    }
                    else
                    {
                        Game1.stats.checkForCookingAchievements();
                    }

                    Game1.player.addItemByMenuIfNecessary(obj);
                }
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.button.bounds = new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
        }

        public int getDescriptionHeight(int width)
        {
            return (int)((double)(ScavengerRecipe.getNumberOfIngredients() * 36) + (double)(int)Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.567")).Y + 21.0);
        }

        public void drawScavengerList(
        SpriteBatch b,
        Vector2 position,
        int width,
        IList<Item> additional_crafting_items)
        {
            if (this.recipeListNeedsUpdate) getRecipeList(this.Reflection);
            int num1 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? 8 : 0;
            b.Draw(Game1.staminaRect, new Rectangle((int)((double)position.X + 8.0), (int)((double)position.Y + 32.0 + (double)Game1.smallFont.MeasureString("Ing!").Y) - 4 - 2 - (int)((double)num1 * 1.5), width - 32, 2), Game1.textColor * 0.35f);
            Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.567"), Game1.smallFont, position + new Vector2(8f, 28f), Game1.textColor * 0.75f, 1f, -1f, -1, -1, 1f, 3);
            for (int index = 0; index < this.currentRecipeList.Count; ++index)
            {
                int num2 = this.currentRecipeList.Values.ElementAt<int>(index);
                int item_index = this.currentRecipeList.Keys.ElementAt<int>(index);
                int itemCount = Game1.player.getItemCount(item_index, 8);
                int num3 = 0;
                int num4 = num2 - itemCount;
                if (additional_crafting_items != null)
                {
                    num3 = Game1.player.getItemCountInList(additional_crafting_items, item_index, 8);
                    if (num4 > 0)
                        num4 -= num3;
                }
                string nameFromIndex = ScavengerRecipe.getNameFromIndex(this.currentRecipeList.Keys.ElementAt<int>(index));
                Color color = num4 <= 0 ? Game1.textColor : Color.Red;
                b.Draw(Game1.objectSpriteSheet, new Vector2(position.X, position.Y + 64f + (float)(index * 64 / 2) + (float)(index * 4)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ScavengerRecipe.getSpriteIndexFromRawIndex(this.currentRecipeList.Keys.ElementAt<int>(index)), 16, 16)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.86f);
                Utility.drawTinyDigits(this.currentRecipeList.Values.ElementAt<int>(index), b, new Vector2(position.X + 32f - Game1.tinyFont.MeasureString(string.Concat((object)this.currentRecipeList.Values.ElementAt<int>(index))).X, (float)((double)position.Y + 64.0 + (double)(index * 64 / 2) + (double)(index * 4) + 21.0)), 2f, 0.87f, Color.AntiqueWhite);
                Vector2 position1 = new Vector2((float)((double)position.X + 32.0 + 8.0), (float)((double)position.Y + 64.0 + (double)(index * 64 / 2) + (double)(index * 4) + 4.0));
                Utility.drawTextWithShadow(b, nameFromIndex, Game1.smallFont, position1, color, 1f, -1f, -1, -1, 1f, 3);
                if (Game1.options.showAdvancedCraftingInformation)
                {
                    position1.X = (float)((double)position.X + (double)width - 40.0);
                    b.Draw(Game1.mouseCursors, new Rectangle((int)position1.X, (int)position1.Y + 2, 22, 26), new Rectangle?(new Rectangle(268, 1436, 11, 13)), Color.White);
                    Utility.drawTextWithShadow(b, string.Concat((object)(itemCount + num3)), Game1.smallFont, position1 - new Vector2(Game1.smallFont.MeasureString((itemCount + num3).ToString() + " ").X, 0.0f), color, 1f, -1f, -1, -1, 1f, 3);
                }
            }
            b.Draw(Game1.staminaRect, new Rectangle((int)position.X + 8, (int)position.Y + num1 + 64 + 4 + this.currentRecipeList.Count * 36, width - 32, 2), Game1.textColor * 0.35f);
        }
    }
}
