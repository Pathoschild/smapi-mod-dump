/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cilekli-link/SDVMods
**
*************************************************/

using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace AutoCrafter
{
    public class CrafterMachine : Chest
    {
        new string Name = "Crafter";
        new string DisplayName = "Crafts things lol";
        public CrafterMachine()
        {
             
        }
        public override bool performUseAction(GameLocation location)
        {
            Logger.Log("h");
            return base.performUseAction(location);
        }

        internal void createMenu()
        {
            throw new NotImplementedException();
        }
    }
}
