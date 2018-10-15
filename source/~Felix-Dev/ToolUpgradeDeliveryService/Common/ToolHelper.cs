using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewMods.ToolUpgradeDeliveryService.Common
{
    internal static class ToolHelper
    {
        public static bool TryParse(string s, out Type result)
        {
            switch (s)
            {
                case Constants.TOOL_AXE:
                    result = typeof(Axe);
                    return true;
                case Constants.TOOL_PICKAXE:
                    result = typeof(Pickaxe);
                    return true;
                case Constants.TOOL_HOE:
                    result = typeof(Hoe);
                    return true;
                case Constants.TOOL_SHEARS:
                    result = typeof(Shears);
                    return true;
                case Constants.TOOL_WATERING_CAN:
                    result = typeof(WateringCan);
                    return true;
            }

            result = null;
            return false;
        }
    }
}
