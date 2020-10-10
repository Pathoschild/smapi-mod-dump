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
using StardewValley;


namespace MineAssist.Framework {
    class CommandCraft : Command {
        public static string name = "Craft";
        public new enum Paramter {
            ItemName,
            ToPosition
        };

        public override void exec(Dictionary<string, string> par) {
            string itemName = "Staircase";
            int toPosition = -1;
            if (par.ContainsKey(Paramter.ItemName.ToString())) {
                itemName = par[Paramter.ItemName.ToString()];
            }
            if (par.ContainsKey(Paramter.ToPosition.ToString())) {
                toPosition = Convert.ToInt32(par[Paramter.ToPosition.ToString()]);
            }
            StardewWrap.fastCraft(itemName, toPosition);
            isFinish = true;
        }
    }
}
