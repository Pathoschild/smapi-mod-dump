/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/M3ales/FishingLogbook
**
*************************************************/

using FishingLogbook.Tracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingLogbook.UI
{
    public class TooltipPatch
    {
        public static void OnTooltipDisplay(Bookcase.Events.ItemTooltipEvent e, FishingLog fishingLog)
        {
            if(e.Item != null)
            if (e.Item.Category == StardewValley.Object.FishCategory)
            {
                    e.AddLine("\r\n" + fishingLog.GetCatchConditionsAsString(e.Item));
            }
        }
    }
}
