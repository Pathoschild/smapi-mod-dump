/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revitalize.Framework.Crafting;
using StardewValley;
using StardustCore.Animations;
using StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons;

namespace Revitalize.Framework.Menus.MenuComponents
{
    public class CraftingRecipeButton
    {
        public Recipe recipe;
        public ItemDisplayButton displayItem;


        public CraftingRecipeButton(Recipe RecipeToCraft, StardustCore.Animations.AnimatedSprite Background,Vector2 Position,Rectangle BoundingBox,float Scale, bool DrawStackNumber, Color DrawColor)
        {
            this.recipe = RecipeToCraft;
            this.displayItem = new ItemDisplayButton(this.recipe.DisplayItem, Background, Position, BoundingBox, Scale, DrawStackNumber, DrawColor);
        }


        /// <summary>
        /// Draws the component to the screen.
        /// </summary>
        /// <param name="B"></param>
        public void draw(SpriteBatch B)
        {
            this.displayItem.draw(B, 0.25f, 1f, false);
        }

        public void draw(SpriteBatch B,Vector2 Position)
        {
            this.displayItem.draw(B,Position,0.25f, 1f, false);
        }

        /// <summary>
        /// Draw the display item with the given alpha
        /// </summary>
        /// <param name="B"></param>
        /// <param name="Alpha"></param>
        public void draw(SpriteBatch B,float Alpha=1f)
        {
            this.displayItem.draw(B, 0.25f, Alpha, false);
        }

        public bool containsPoint(int x, int y)
        {
            return this.displayItem.Contains(x, y);
        }

        public void craftItem()
        {
            this.recipe.craft();
        }

        public void craftItem(IList<Item> fromInventory, IList<Item> toInventory)
        {
            this.recipe.craft(ref fromInventory, ref toInventory, false, false);
        }
    }
}
