using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace ChestNaming.UI
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
            this.content = new FrameContent(x, y, color, FrameAnchor.TopLeft, parentAnchor, parent: null, xPadding: 20, yPadding: 20)
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
        public FrameContent content;
        public ShadowText header;
        public ShadowText body;
    }
}
