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
    internal class ColoredRectangleComponent : BaseMenuComponent
    {
        /*********
        ** Accessors
        *********/
        public Color Color;


        /*********
        ** Public methods
        *********/
        public ColoredRectangleComponent(Rectangle area, Color color)
        {
            this.SetScaledArea(area);
            this.Color = color;
        }

        public override void Draw(SpriteBatch b, Point o)
        {
            b.Draw(Game1.staminaRect, new Rectangle(this.Area.X + o.X, this.Area.Y + o.Y, this.Area.Width, this.Area.Height), this.Color);
        }
    }
}
