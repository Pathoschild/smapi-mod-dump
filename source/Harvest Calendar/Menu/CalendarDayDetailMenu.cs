/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LonerAxl/Stardew_HarvestCalendar
**
*************************************************/

using HarvestCalendar.Framework;
using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace HarvestCalendar.Menu
{
    internal class CalendarDayDetailMenu : ScrollableMenu
    {

        private CalendarDayItem _item;
        private Billboard _billboard;

        public CalendarDayDetailMenu(CalendarDayItem item, Billboard billboard) : base(I18n.UI_CalendarDayDetail_Title())
        {
            _item = item;
            _billboard = billboard;
            SetOptions();
        }

        protected override void SetOptions()
        {
            var slotWidth = _optionSlots[0].bounds.Width;
            _options.Clear();
            string cur_location = string.Empty;
            _item.locationCrops.ForEach(lc =>
            {
                if (cur_location != lc.Item1) 
                {
                    cur_location = lc.Item1;
                    _options.Add(new CalendarDayDetailOption(
                        location: lc.Item1,
                        slotWidth: slotWidth
                    ));
                }

                _options.Add(new CalendarDayDetailOption(
                    location: lc.Item1,
                    crop: ItemRegistry.GetDataOrErrorItem(lc.Item2).DisplayName,
                    slotWidth: slotWidth,
                    count: lc.Item3
                ));
            });


        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.menuButton.Contains(new InputButton(key)))
            {
                Game1.activeClickableMenu = _billboard;
            }

        }
    }
}
