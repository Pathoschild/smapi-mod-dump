using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderExample.Framework.Drawers
{
    class Menus
    {

        public static void craftingPageDraw(StardewValley.Menus.CraftingPage menu, SpriteBatch b)
        {

            int craftingPage = (int)Class1.GetInstanceField(typeof(StardewValley.Menus.CraftingPage), menu, "currentCraftingPage");
            bool cooking = (bool)Class1.GetInstanceField(typeof(StardewValley.Menus.CraftingPage), menu, "cooking");
            if (cooking)
                Game1.drawDialogueBox(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.width, menu.height, false, true, (string)null, false);

            Class1.getInvokeMethod(menu, "drawHorizontalPartition", new object[] {

                b, menu.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256, false
            });


            menu.inventory.draw(b);
            if (menu.trashCan != null)
            {
                menu.trashCan.draw(b);
                b.Draw(Game1.mouseCursors, new Vector2((float)(menu.trashCan.bounds.X + 60), (float)(menu.trashCan.bounds.Y + 40)), new Rectangle?(new Rectangle(686, 256, 18, 10)), Color.White, menu.trashCanLidRotation, new Vector2(16f, 10f), 4f, SpriteEffects.None, 0.86f);
            }
            //b.End();
            //b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null);
            IList<Item> fridge = (IList<Item>)Class1.getInvokeMethod(menu, "fridge", new object[] { });
            foreach (ClickableTextureComponent key in menu.pagesOfCraftingRecipes[craftingPage].Keys)
            {

                if (key.hoverText.Equals("ghosted"))
                    key.draw(b, Color.Black * 0.35f, 0.89f);
                else if (!menu.pagesOfCraftingRecipes[craftingPage][key].doesFarmerHaveIngredientsInInventory(cooking ? fridge : (IList<Item>)null))
                {
                    key.draw(b, Color.LightGray * 0.4f, 0.89f);
                }
                else
                {
                    key.draw(b);
                    if (menu.pagesOfCraftingRecipes[craftingPage][key].numberProducedPerCraft > 1)
                        NumberSprite.draw(menu.pagesOfCraftingRecipes[craftingPage][key].numberProducedPerCraft, b, new Vector2((float)(key.bounds.X + 64 - 2), (float)(key.bounds.Y + 64 - 2)), Color.Red, (float)(0.5 * ((double)key.scale / 4.0)), 0.97f, 1f, 0, 0);
                }
            }
            //b.End();
            //b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null);

            Item hoverItem = (Item)Class1.GetInstanceField(menu.GetType(), menu, "hoverItem");
            Item heldItem = (Item)Class1.GetInstanceField(menu.GetType(), menu, "heldItem");
            Item lastCookingHover = (Item)Class1.GetInstanceField(menu.GetType(), menu, "lastCookingHover");
            string hoverText = (string)Class1.GetInstanceField(menu.GetType(), menu, "hoverText");
            string hoverTitle = (string)Class1.GetInstanceField(menu.GetType(), menu, "hoverTitle");

            CraftingRecipe hoverRecipe = (CraftingRecipe)Class1.GetInstanceField(menu.GetType(), menu, "hoverRecipe");

            if (hoverItem != null)
                IClickableMenu.drawToolTip(b, hoverText, hoverTitle, hoverItem, heldItem != null, -1, 0, -1, -1, (CraftingRecipe)null, -1);
            else if (!string.IsNullOrEmpty(hoverText))
                IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont, heldItem != null ? 64 : 0, heldItem != null ? 64 : 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
            if (heldItem != null)
                heldItem.drawInMenu(b, new Vector2((float)(Game1.getOldMouseX() + 16), (float)(Game1.getOldMouseY() + 16)), 1f);

            //(menu as IClickableMenu).draw(b);
            if (menu.downButton != null && craftingPage < menu.pagesOfCraftingRecipes.Count - 1)
                menu.downButton.draw(b);
            if (menu.upButton != null && craftingPage > 0)
                menu.upButton.draw(b);
            //if (cooking)
            //drawMouse(b);
            if (hoverRecipe == null)
                return;
            SpriteBatch b1 = b;
            string text = " ";
            SpriteFont smallFont = Game1.smallFont;
            int xOffset = heldItem != null ? 48 : 0;
            int yOffset = heldItem != null ? 48 : 0;
            int moneyAmountToDisplayAtBottom = -1;
            string displayName = hoverRecipe.DisplayName;
            int healAmountToDisplay = -1;
            string[] buffIconsToDisplay;
            if (cooking && lastCookingHover != null)
            {
                if (Game1.objectInformation[(int)((lastCookingHover as StardewValley.Object).parentSheetIndex)].Split('/').Length > 7)
                {
                    buffIconsToDisplay = Game1.objectInformation[(int)((lastCookingHover as StardewValley.Object).parentSheetIndex)].Split('/')[7].Split(' ');
                    goto label_32;
                }
            }
            buffIconsToDisplay = (string[])null;
            label_32:
            Item lastCookingHover2 = lastCookingHover;
            int currencySymbol = 0;
            int extraItemToShowIndex = -1;
            int extraItemToShowAmount = -1;
            int overrideX = -1;
            int overrideY = -1;
            double num = 1.0;
            CraftingRecipe hoverRecipe2 = hoverRecipe;
            IClickableMenu.drawHoverText(b1, text, smallFont, xOffset, yOffset, moneyAmountToDisplayAtBottom, displayName, healAmountToDisplay, buffIconsToDisplay, lastCookingHover2, currencySymbol, extraItemToShowIndex, extraItemToShowAmount, overrideX, overrideY, (float)num, hoverRecipe2);
        }



    }
}
