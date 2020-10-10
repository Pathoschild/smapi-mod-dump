/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/M3ales/RelationshipTooltips
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace RelationshipTooltips.UI
{
    /// <summary>
    /// A wrapper using Frame's positioning and anchoring logic to draw a tooltip
    /// </summary>
    public class Tooltip : Frame
    {
        public Tooltip(int x, int y, Color color, FrameAnchor anchor = FrameAnchor.TopLeft, FrameAnchor parentAnchor = FrameAnchor.TopLeft, List<IFrameDrawable> components = null, Frame parent = null,string header = "", string text = "") : base(x, y, color, anchor, parentAnchor, components, parent)
        {
            this.header = new ShadowText(text, Game1.dialogueFont, Game1.textColor);
            this.body = new ShadowText(text, Game1.smallFont, Game1.textColor);
            this.content = new TooltipContent(x, y, color, FrameAnchor.TopLeft, parentAnchor, parent: null, xPadding: 20, yPadding: 20)
            {
                components = new List<IFrameDrawable>()
                {
                    this.header,
                    this.body
                }
            };
            this.components = new List<IFrameDrawable>()
            {
                new TextureBox(color),
                content
            };
        }
        public TooltipContent content;
        public ShadowText header;
        public ShadowText body;
    }
}
