using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChestNaming.UI
{
    public class FrameContent : Frame, IFrameDrawable
    {
        public FrameContent(int x, int y, Color color, FrameAnchor anchor = FrameAnchor.TopLeft, FrameAnchor parentAnchor = FrameAnchor.TopLeft, List<IFrameDrawable> components = null, Frame parent = null, int xPadding = 0, int yPadding = 0) : base(x, y, color, anchor, parentAnchor, components, parent)
        {
            this.xPadding = xPadding;
            this.yPadding = yPadding;
        }

        public int xPadding;
        public int yPadding;

        void IFrameDrawable.Draw(SpriteBatch b, int x, int y, Frame parentFrame)
        {
            this.localX = x + xPadding;
            this.localY = y + yPadding;
            this.Draw(b, null);
        }

        int IFrameDrawable.SizeX => this.Width + (xPadding * 2);

        int IFrameDrawable.SizeY => this.Height + (yPadding * 2);
    }
}
