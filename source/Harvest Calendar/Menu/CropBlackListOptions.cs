/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LonerAxl/Stardew_HarvestCalendar
**
*************************************************/

using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HarvestCalendar.Menu
{
    internal class CropBlackListOptions:OptionsElement
    {

        private readonly Rectangle _removeButtonSprite;
        private Rectangle _removeButtonBounds;
        private Action _toggle;

        public CropBlackListOptions(int slotWidth, string label, Action toggle) : base(label)
        {
            _toggle = toggle;
            _removeButtonSprite = new(269, 471, 14, 15);
            _removeButtonBounds = new(slotWidth - 28 * Game1.pixelZoom, -1 + Game1.pixelZoom * 3, 14 * Game1.pixelZoom, 15 * Game1.pixelZoom);
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (!_removeButtonBounds.Contains(x, y)) return;
            
            _toggle();
        }

        public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
        {
            Utility.drawTextWithShadow(b, label, Game1.dialogueFont, new Vector2(bounds.X + slotX, bounds.Y + slotY), Game1.textColor, 1f, 0.15f);
            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(_removeButtonBounds.X + slotX, _removeButtonBounds.Y + slotY), _removeButtonSprite, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom * 0.75f, false, 0.15f);
        }


    }
}
