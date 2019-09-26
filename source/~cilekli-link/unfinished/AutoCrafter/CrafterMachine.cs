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
