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
