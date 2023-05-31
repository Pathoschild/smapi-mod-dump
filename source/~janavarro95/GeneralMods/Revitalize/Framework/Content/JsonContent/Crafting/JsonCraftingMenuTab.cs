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
using Omegasis.Revitalize.Framework.Content.JsonContent;
using Omegasis.Revitalize.Framework.Content.JsonContent.Animations;
using Omegasis.StardustCore.Animations;
using Omegasis.StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons;

namespace Omegasis.Revitalize.Framework.Crafting.JsonContent
{
    /// <summary>
    /// Contains information regarding crafting tabs to be displayed for the crafting menu in json formatting.
    /// </summary>
    public class JsonCraftingMenuTab
    {
        public string craftingTabName;
        public bool isDefaultTab;
        public Rectangle craftingTabBoundingBox;
        public float scale;
        public JsonAnimationManager animationManager;

        public JsonCraftingMenuTab()
        {
            this.craftingTabName = "";
            this.isDefaultTab = false;
            this.craftingTabBoundingBox = new Rectangle();
            this.scale = 1f;
            this.animationManager = new JsonAnimationManager();
        }


        /// <summary>
        /// Gets an animated sprite from this JsonCraftingMenuTab definition.
        /// </summary>
        /// <returns></returns>
        public virtual AnimatedSprite getAnimatedSprite()
        {
            return new AnimatedSprite(this.craftingTabName, new Vector2(), this.animationManager.toAnimationManager(), Color.White);
        }

        public virtual AnimatedButton getAnimatedButton()
        {
            return new AnimatedButton(this.getAnimatedSprite(), this.craftingTabBoundingBox, this.scale);
        }
    }
}
