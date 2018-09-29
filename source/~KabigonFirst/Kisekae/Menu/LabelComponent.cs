using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace Kisekae.Menu {
    class LabelComponent : ClickableComponent, IAutoComponent {
        public string m_name { get { return name; } set { name = value; } }
        public bool m_visible { get { return visible; } set { visible = value; } }
        public bool m_isTitle = false;
        public int m_baseX;
        public int m_baseY;
        public Color m_fontFolor = Game1.textColor;
        public SpriteFont m_font = Game1.smallFont;

        public LabelComponent(int baseX, int baseY, int width, int height, string label, string name = "") : base(new Rectangle(baseX, baseY, width, height), label, name) {
            m_baseX = baseX;
            m_baseY = baseY;
        }
        public LabelComponent(Rectangle r, string label, string name = "") : base(r, name, label) {
            m_baseX = r.X;
            m_baseY = r.Y;
        }
        public LabelComponent(int x, int y, string label, string name = "") : base(new Rectangle(x,y,1,1), name, label) {
            m_baseX = x;
            m_baseY = y;
        }

        public void draw(SpriteBatch b) {
            if (m_isTitle) {
                SpriteText.drawString(b, label, bounds.X, bounds.Y);
            } else {
                Utility.drawTextWithShadow(b, label, m_font, new Vector2(bounds.X, bounds.Y), m_fontFolor);
            }
        }

        public void centerLabelX(int adjust = 0) {
            float texeLength = Game1.smallFont.MeasureString(label).X;
            bounds.X = (int)(m_baseX + bounds.Width / 2 + adjust - texeLength / 2);
        }

        public void centerLabelY(int adjust = 0) {
            float texeLength = Game1.smallFont.MeasureString(label).Y;
            bounds.X = (int)(m_baseY + bounds.Height / 2 + adjust - texeLength / 2);
        }

        public void rightCenterLabelX(int adjust = 0) {
            float texeLength = Game1.smallFont.MeasureString(label).X;
            bounds.X = (int)(m_baseX + bounds.Width / 2 + adjust - texeLength);
        }
    }
}
