using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using StardewValley;

namespace ChestNaming.UI
{
    /// <summary>
    /// Main layout class, provides layout, anchoring, and relative positioning. Can be used to create content aware (resizable) UI elements relatively easily. Limited for the time being.
    /// </summary>
    public abstract class Frame
    {
        public int localX;
        public int localY;
        public int GlobalX
        {
            get
            {
                if (parent == null)
                    return localX - AnchorPointX(this, anchor);
                //anchor logic here
                return parent.GlobalX + localX - AnchorPointX(this, anchor) + AnchorPointX(parent, parentAnchor);
            }
        }
        public int GlobalY
        {
            get
            {
                if (parent == null)
                    return localY - AnchorPointY(this, anchor);
                //anchor logic here
                return parent.GlobalY + localY - AnchorPointY(this, anchor) + AnchorPointY(parent, parentAnchor);
            }
        }
        public int AnchorPointX(Frame frame, FrameAnchor anchor)
        {
            if (frame == null)
                return 0;
            switch (anchor)
            {
                case FrameAnchor.TopMid:
                case FrameAnchor.MidMid:
                case FrameAnchor.BottomMid:
                    {
                        return frame.Width / 2;
                    }
                case FrameAnchor.TopRight:
                case FrameAnchor.MidRight:
                case FrameAnchor.BottomRight:
                    {
                        return frame.Width;
                    }
                case FrameAnchor.TopLeft:
                case FrameAnchor.MidLeft:
                case FrameAnchor.BottomLeft:
                    {
                        return 0;
                    }
            }
            return 0;
        }
        public int AnchorPointY(Frame frame, FrameAnchor anchor)
        {
            if (frame == null)
                return 0;
            switch (anchor)
            {
                case FrameAnchor.TopMid:
                case FrameAnchor.TopLeft:
                case FrameAnchor.TopRight:
                    {
                        return 0;
                    }
                case FrameAnchor.MidRight:
                case FrameAnchor.MidMid:
                case FrameAnchor.MidLeft:
                    {
                        return frame.Height / 2;
                    }
                case FrameAnchor.BottomLeft:
                case FrameAnchor.BottomRight:
                case FrameAnchor.BottomMid:
                    {
                        return frame.Height;
                    }
            }
            return 0;
        }
        public Frame(int x, int y) : this(x, y, Color.White)
        {

        }
        public Frame(int x, int y, Color color, FrameAnchor anchor = FrameAnchor.TopLeft, FrameAnchor parentAnchor = FrameAnchor.TopLeft, List<IFrameDrawable> components = null, Frame parent = null)
        {
            children = new List<Frame>();
            if (components == null)
                components = new List<IFrameDrawable>();
            localX = x;
            localY = y;
            this.color = color;
            this.parent = parent;
            this.anchor = anchor;
            this.parentAnchor = parentAnchor;
        }
        public int Height
        {
            get
            {
                if (components != null)
                    if (components.Count > 0)
                        return components.Sum(c => c.SizeY);
                return 0;
            }
        }
        public int Width
        {
            get
            {
                if (components != null)
                    if (components.Count > 0)
                        return components.Max(c=> c.SizeX);
                return 0;
            }
        }
        Color color;
        public Frame parent;
        public List<Frame> children;
        public List<IFrameDrawable> components;
        public FrameAnchor parentAnchor = FrameAnchor.TopLeft;
        public FrameAnchor anchor = FrameAnchor.TopLeft;
        public void AddToChildren(Frame f)
        {
            f.parent = this;
            children.Add(f);
        }
        public void Draw(SpriteBatch b, Frame parent)
        {
            FrameAnchor temp = anchor;
            FrameAnchor tempParent = parentAnchor;
            int internalYOffset = 0;//Change offsetting here if you want to do interesting layouts
                                    //layout is by default just vertical elements
            if (!CheckFrameOnScreen())
            {
                anchor = Flip(temp);
                parentAnchor = Flip(temp);
                if(!CheckFrameOnScreen(true))
                {
                    //problem
                    anchor = temp;
                    parentAnchor = tempParent;
                }
            }
            foreach (IFrameDrawable d in components)
            {
                d.Draw(b, GlobalX, GlobalY + internalYOffset, this);
                internalYOffset += d.SizeY;
            }
            foreach (Frame f in children)
            {
                f.Draw(b, this);
            }
            anchor = temp;
            parentAnchor = tempParent;
        }
        public int edgeTolerance = 30;
        /// <summary>
        /// Checks if a frame is within screen bounds, making use of edgeTolerance to pad the screen edges
        /// </summary>
        /// <param name="reversed">If the tooltip has been flipped, will likely be helpful if/when vertical flipping is a thing</param>
        /// <returns>If the frame's bounds are within the screen + edgeTolerance</returns>
        public bool CheckFrameOnScreen(bool reversed = false)
        {
            Rectangle screen = Game1.graphics.GraphicsDevice.Viewport.Bounds;
            int farEdgeX = reversed ? GlobalX - Width : GlobalX + Width;
            if (GlobalX < -edgeTolerance || farEdgeX > screen.Width + edgeTolerance)
                return false;
            return true;
        }
        /// <summary>
        /// Reflection of FrameAnchor about the Y axis.
        /// </summary>
        /// <param name="f">The FrameAnchor to reflect</param>
        /// <returns>The Reflection about the Y axis.</returns>
        public FrameAnchor Flip(FrameAnchor f)
        {
            switch(f)
            {
                case FrameAnchor.BottomLeft:
                    {
                        return FrameAnchor.BottomRight;
                    }
                case FrameAnchor.MidLeft:
                    {
                        return FrameAnchor.MidRight;
                    }
                case FrameAnchor.TopLeft:
                    {
                        return FrameAnchor.TopRight;
                    }
                case FrameAnchor.BottomRight:
                    {
                        return FrameAnchor.BottomLeft;
                    }
                case FrameAnchor.MidRight:
                    {
                        return FrameAnchor.MidLeft;
                    }
                case FrameAnchor.TopRight:
                    {
                        return FrameAnchor.TopLeft;
                    }
            }
            return f;
        }
    }
    public enum FrameAnchor
    {
        TopLeft, TopMid, TopRight,
        MidLeft, MidMid, MidRight,
        BottomLeft, BottomMid, BottomRight
    }
}
