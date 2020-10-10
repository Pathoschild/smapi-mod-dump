using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace EconomyMod.Interface.PageContent
{
    public class ContentElementHeaderText : IContentElement
    {
        private const int DefaultX = 8;
        private const int DefaultY = 4;
        private const int DefaultPixelSize = 9;
        private Rectangle _bounds;
        private string _label;
        private int _whichOption;
        protected bool _canClick = true;
        private Func<string> dynamicLabel;

        public Rectangle Bounds { get { return _bounds; } }

        public ContentElementHeaderText(string label)
            : this(label, -1, -1, DefaultPixelSize * Game1.pixelZoom, DefaultPixelSize * Game1.pixelZoom)
        {

        }

        public ContentElementHeaderText(string label, int x, int y, int width, int height, int whichOption = -1)
        {
            if (x < 0)
                x = DefaultX * Game1.pixelZoom;

            if (y < 0)
                y = DefaultY * Game1.pixelZoom;

            _bounds = new Rectangle(x + 16, y, width, height);
            _label = label;
            _whichOption = whichOption;
        }

        public ContentElementHeaderText(Func<string> p) : this("ignored", -1, -1, DefaultPixelSize * Game1.pixelZoom, DefaultPixelSize * Game1.pixelZoom)
        {
            dynamicLabel = p;
        }

        public virtual void ReceiveLeftClick(int x, int y)
        {

        }

        public virtual void LeftClickHeld(int x, int y)
        {

        }

        public virtual void LeftClickReleased(int x, int y)
        {

        }

        public virtual void ReceiveKeyPress(Keys key)
        {

        }

        public virtual void draw(SpriteBatch batch, int slotX, int slotY)
        {
            string label = dynamicLabel == null ? _label : dynamicLabel();
            if (_whichOption < 0)
            {
                SpriteText.drawString(batch, label, slotX + _bounds.X, slotY + _bounds.Y + Game1.pixelZoom * 3, 999, -1, 999, 1, 0.1f);
            }
            else
            {
                Utility.drawTextWithShadow(batch,
                    label,
                    Game1.dialogueFont,
                    new Vector2(slotX + _bounds.X + _bounds.Width + Game1.pixelZoom * 2, slotY + _bounds.Y),
                    _canClick ? Game1.textColor : Game1.textColor * 0.33f,
                    1f,
                    0.1f);
            }
        }
    }
}
