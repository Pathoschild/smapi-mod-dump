using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace Kisekae.Menu {
    class ClickableTextureButton : ClickableTextureComponent, IAutoComponent, IHoverable {
        public string m_name { get { return name; } set { name = value; } }
        public bool m_visible { get { return visible; } set { visible = value;} }
        public float m_hoverScale { get; set; } = 0f;
        public int m_par = 0;

        public ClickableTextureButton(string name, Rectangle bounds, Texture2D texture, Rectangle sourceRect, float scale, bool drawShadow = false) : base(bounds, texture, sourceRect, scale, drawShadow) {
            m_name = name;
        }
        public ClickableTextureButton(Rectangle bounds, Texture2D texture, Rectangle sourceRect, float scale, bool drawShadow = false) : base(bounds, texture, sourceRect, scale, drawShadow) {
        }
        public ClickableTextureButton(string name, Rectangle bounds, string label, string hoverText, Texture2D texture, Rectangle sourceRect, float scale, bool drawShadow = false) : base(name, bounds, label, hoverText, texture, sourceRect, scale, drawShadow) {
        }
    }
}
