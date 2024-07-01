/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LonerAxl/Stardew_HarvestCalendar
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestCalendar.Menu
{
    internal class CalendarDayDetailOption:OptionsElement
    {
        private string _location;
        private string _crop;
        private int _count;
        private int _slotWidth;
        public CalendarDayDetailOption(int slotWidth, string location, string crop="", int count = 0) : base(location+crop)
        {
            _location= location;
            _crop = crop;
            _count = count;
            _slotWidth = slotWidth;
        }


        public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
        {
            if (_crop == "" && _count == 0)
            {
                Utility.drawTextWithShadow(b, _location, Game1.dialogueFont, new Vector2(bounds.X + slotX, bounds.Y + slotY), Game1.textColor, 1f, 0.15f);
            }
            else 
            {
                Utility.drawTextWithShadow(b, $"    {_crop}: {_count}", Game1.dialogueFont, new Vector2(bounds.X + slotX, bounds.Y + slotY), Game1.textColor, 1f, 0.15f);
            }


        }
    }
}
