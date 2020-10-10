/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KabigonFirst/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampfireCooking.Config {
    class ModConfig {
        public bool isEnable = true;
        public HashSet<string> cookableItemNames = new HashSet<string> {
            "Campfire"
        };
        public HashSet<string> testedPossiblecookableItems = new HashSet<string> {
            "Wooden Brazier", "Stone Brazier", "Gold Brazier", "Carved Brazier", "Stump Brazier", "Barrel Brazier", "Skull Brazier", "Marble Brazier", "Brick Fireplace", "Iridium Fireplace", "Monster Fireplace", "Stone Fireplace", "Stove Fireplace"
        };
    }
}
