using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace TravellingMerchantInventory
{
    class ModConfig
    {
        public bool buyFromMenuPage { get; set; }
        public SButton button { get; set; }

        public ModConfig()
        {
            this.buyFromMenuPage = false;
            this.button = SButton.F1;
        }
    }
}
