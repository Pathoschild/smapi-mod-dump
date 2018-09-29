using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace StarDustCore.Messages
{
    public class HudWithoutIcon
    {
        public static void showGlobalMessage(string s)
        {
            Game1.showGlobalMessage(s);
        }

    }
}
