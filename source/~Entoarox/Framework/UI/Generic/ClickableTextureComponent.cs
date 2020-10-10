/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework.UI
{
    public class ClickableTextureComponent : BaseInteractiveMenuComponent
    {
        /*********
        ** Fields
        *********/
        protected bool ScaleOnHover;


        /*********
        ** Accessors
        *********/
        public event ClickHandler Handler;


        /*********
        ** Public methods
        *********/
        public ClickableTextureComponent(Rectangle area, Texture2D texture, ClickHandler handler = null, Rectangle? crop = null, bool scaleOnHover = true)
            : base(area, texture, crop)
        {
            if (handler != null)
                this.Handler += handler;
            this.ScaleOnHover = scaleOnHover;
        }

        public override void HoverIn(Point p, Point o)
        {
            Game1.playSound("Cowboy_Footstep");
            if (!this.ScaleOnHover)
                return;
            this.Area.X -= 2;
            this.Area.Y -= 2;
            this.Area.Width += 4;
            this.Area.Height += 4;
        }

        public override void HoverOut(Point p, Point o)
        {
            if (!this.ScaleOnHover)
                return;
            this.Area.X += 2;
            this.Area.Y += 2;
            this.Area.Width -= 4;
            this.Area.Height -= 4;
        }

        public override void LeftClick(Point p, Point o)
        {
            Game1.playSound("bigDeSelect");
            this.Handler?.Invoke(this, this.Parent, this.Parent.GetAttachedMenu());
        }
    }
}
