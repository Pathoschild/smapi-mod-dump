/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.GUI.Menus
{
    public class MagicAltarTab
    {
        public const int TEXTURE_WIDTH = 1024;
        public const int TEXTURE_HEIGHT = 1024;

        Texture2D background;
        Texture2D icon;
        int width;
        int height;
        int startX;
        int startY;
        int index;

        string name;

        public MagicAltarTab(string name, Texture2D background, Texture2D icon, int width, int height, int startX, int startY, int index) 
        { 
            this.background = background;
            this.icon = icon;
            this.width = width;
            this.height = height;
            this.startX = startX;
            this.startY = startY;
            this.index = index;
            this.name = name;
        }

        /// <summary>
        /// Returns the location of the background texture for this skill tree.
        /// </summary>
        /// <param name="access">The registry access.</param>
        /// <returns>The location of the background texture.</returns>
        public Texture2D GetBackground()
        {
            return background;
        }

        /// <summary>
        /// Returns the location of the icon texture for this skill tree.
        /// </summary>
        /// <param name="access">The registry access.</param>
        /// <returns>The location of the icon texture.</returns>
        public Texture2D GetIcon()
        {
            return icon;
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }

        public int GetStartX()
        {
            return startX;
        }

        public int GetStartY()
        {
            return startY;
        }

        public int GetIndex()
        {
            return index;
        }

        public string GetName()
        {
            return name;
        }

        public static MagicAltarTab create(string name, Texture2D background, Texture2D icon, int width, int height, int startX, int startY, int index)
        {
            return new MagicAltarTab(name, background, icon, width, height, startX, startY, index);
        }
    }
}
