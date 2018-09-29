using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Patches.GameLocationPatch
{
    //[HarmonyPatch(typeof(GameLocation))]
    //[HarmonyPatch("startEvent")]
    class startEventPatch
    {
        public static void Prefix(ref Event evt)
        {
            if (Game1.whichFarm > 4)
            {
                int x = Memory.loadedFarm.farmHousePorchX() - 1;
                int y = Memory.loadedFarm.farmHousePorchY() + 2;
                if (evt.id == 1590166)
                {
                    evt.eventCommands[2] = "farmer 64 15 2 Marnie 65 16 0 cat " + x.ToString() + " " + y.ToString() + " 2";
                }
                else if (evt.id == 897405)
                {
                    evt.eventCommands[2] = "farmer 64 15 2 Marnie 65 16 0 dog " + x.ToString() + " " + y.ToString() + " 2";
                }
            }
        }
    }
}
