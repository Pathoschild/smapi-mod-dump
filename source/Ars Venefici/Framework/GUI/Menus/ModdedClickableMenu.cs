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
    public abstract class ModdedClickableMenu : IClickableMenu
    {

        /// <summary>The callback to invoke when the birthday value changes.</summary>
        private Action<string, int> OnChanged;

        public ModdedClickableMenu(int x, int y, int width, int height, bool showUpperRightCloseButton = false)
            : base(x, y, width, height, showUpperRightCloseButton)
        {
            UpdateMenu();
        }

        public ModdedClickableMenu(int x, int y, int width, int height, Action<string, int> onChanged, bool showUpperRightCloseButton = false)
            : base(x, y, width, height, showUpperRightCloseButton)
        {
            UpdateMenu(onChanged);
        }

        public virtual void UpdateMenu(Action<string, int> OnChanged)
        {
            this.OnChanged = OnChanged;
            SetUpPositions();
        }


        public virtual void UpdateMenu()
        {
            SetUpPositions();
        }

        /// <summary>Regenerate the UI.</summary>
        protected abstract void SetUpPositions();

        /// <summary>
        /// Changes Scale When A Component Is Hovered.
        /// </summary>
        /// <param name="component">Current Component</param>
        /// <param name="x">X Position Of The Mouse</param>
        /// <param name="y">Y Position Of The Mouse</param>
        /// <param name="min">The Minimum Scale</param>
        /// <param name="max">The Maximum Scale</param>
        public void ChangeHoverActionScale(ClickableTextureComponent component, int x, int y, float min, float max)
        {
            if (component.containsPoint(x, y))
                component.scale = Math.Min(component.scale + min, component.baseScale + max);
            else
                component.scale = Math.Max(component.scale - min, component.baseScale);
        }
    }
}
