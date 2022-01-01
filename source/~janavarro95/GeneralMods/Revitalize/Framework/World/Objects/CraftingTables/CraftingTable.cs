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
using Revitalize.Framework.World.Objects.InformationFiles;
using Revitalize.Framework.Crafting;
using StardewValley;

namespace Revitalize.Framework.World.Objects.CraftingTables
{
    public class CraftingTable : CustomObject
    {
        public string craftingBookName;


        public CraftingTable()
        {

        }

        public CraftingTable(BasicItemInformation Info,string CraftingRecipeBookName):base(Info)
        {
            this.craftingBookName = CraftingRecipeBookName;
        }

        public CraftingTable(BasicItemInformation Info,Vector2 TilePosition ,string CraftingRecipeBookName) : base(Info,TilePosition)
        {
            this.craftingBookName = CraftingRecipeBookName;
        }

        /// <summary>
        /// When the chair is right clicked ensure that all pieces associated with it are also rotated.
        /// </summary>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool rightClicked(Farmer who)
        {
            if (CraftingRecipeBook.CraftingRecipesByGroup.ContainsKey(this.craftingBookName))
            {
                CraftingRecipeBook.CraftingRecipesByGroup[this.craftingBookName].openCraftingMenu();
                return true;
            }
            else
            {
                return true;
            }
        }


        public override Item getOne()
        {
            CraftingTable component = new CraftingTable(this.getItemInformation().Copy(),this.craftingBookName);
            return component;
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (x <= -1)
            {
                return;
            }
            if (this.AnimationManager == null)
            {
                spriteBatch.Draw(this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.AnimationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(y * Game1.tileSize) / 10000f));
            }
            else
            {
                float addedDepth = 0;
                this.AnimationManager.draw(spriteBatch, this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.TileLocation.X * Game1.tileSize), this.TileLocation.Y * Game1.tileSize)), new Rectangle?(this.AnimationManager.getCurrentAnimationFrameRectangle()), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)((this.TileLocation.Y + addedDepth) * Game1.tileSize) / 10000f) + .00001f);
            }
        }
    }
}
