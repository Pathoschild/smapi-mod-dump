/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.GUI.Menus
{
    public abstract class MagicAltarTabRenderer
    {
        protected readonly int textureHeight;
        protected readonly int textureWidth;
        protected readonly MagicAltarTab magicAltarTab;
        protected readonly MagicAltarMenu parent;
        protected int screenWidth;
        protected int screenHeight;
        protected int width;
        protected int height;
        protected int posX;
        protected int posY;

        public Rectangle bounds;

        protected MagicAltarTabRenderer(MagicAltarTab magicOrbTab, MagicAltarMenu parent)
        {
            this.textureHeight = magicOrbTab.GetHeight();
            this.textureWidth = magicOrbTab.GetWidth();
            this.magicAltarTab = magicOrbTab;
            this.parent = parent;
        }

        public void Init(int width, int height, int screenWidth, int screenHeight, int posX, int posY)
        {
            this.width = width;
            this.height = height;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.posX = posX;
            this.posY = posY;

            this.bounds = new Rectangle(posX, posY, width, height);

            Init();
        }

        public void Draw(SpriteBatch spriteBatch, int pMouseX, int pMouseY, float pPartialTicks)
        {
            //pMouseX -= posX;
            //pMouseY -= posY;

            RenderBg(spriteBatch, pMouseX, pMouseY, pPartialTicks);
            RenderFg(spriteBatch, pMouseX, pMouseY, pPartialTicks);
        }

        /**
         * Render the background in this method.
         */
        protected abstract void RenderBg(SpriteBatch spriteBatch, int mouseX, int mouseY, float partialTicks);

        /**
         * Render the foreground in this method.
         */
        protected abstract void RenderFg(SpriteBatch spriteBatch, int mouseX, int mouseY, float partialTicks);


        protected Farmer GetPlayer()
        {
            return Game1.player;
        }

        protected virtual void Init()
        {

        }

        public virtual void MouseHover(float mouseX, float mouseY)
        {

        }

        public virtual void MouseClicked(float mouseX, float mouseY)
        {

        }
    }
}
