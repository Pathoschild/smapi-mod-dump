/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unlockable_Bundles.Lib.ShopTypes
{
    public class ClickableRequirementTexture : ClickableTextureComponent
    {
        public string ReqKey;
        public string ReqItemId;
        public int ReqValue;

        public ClickableRequirementTexture(string name, Rectangle bounds, string label, string hoverText, Texture2D texture, Rectangle sourceRect, float scale, bool drawShadow = false) : base(name, bounds, label, hoverText, texture, sourceRect, scale, drawShadow)
        {
        }

        public ClickableRequirementTexture(Rectangle bounds, Texture2D texture, Rectangle sourceRect, float scale, bool drawShadow = false) : base("", bounds, "", "", texture, sourceRect, scale, drawShadow)
        {
        }
    }
}
