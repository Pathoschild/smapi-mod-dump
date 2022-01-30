/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/xynerorias/pierre-roulette-shop-SV
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pierresroulette
{
    public class ModConfig
    {
        // Wether or not the mod affects the JojaMart
        //Default: false
        public bool JojaEnabled { get; set; } = false;

        // The shop owners to be affected by the mod. Their name must be their ShopMenu.portaitPerson.Name
        // Default: ["Pierre"]
        public string[] Owners { get; set; } = { "Pierre" };

        // Cycle between "OnlySeeds" to affect only the seeds, "OnlySaplings" to affect only the saplings or "Both" to affect them both
        // Default: "Both" 
        public string Mode { get; set; } = "Both";

        // How much seeds does the shops have in stock every day. Set to 0 to disable. Ignored if mode is "OnlySaplings"
        // Default: 5
        public int SeedStock { get; set; } = 5;

        //How much saplings does the shop have in stock every day. Set to 0 to disable. Ignored if mode is "OnlyCrops"
        // Default: 2
        public int SaplingStock { get; set; } = 2;

    }
}
