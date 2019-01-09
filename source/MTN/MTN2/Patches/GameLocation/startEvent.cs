using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.GameLocationPatches
{
    public class startEventPatch
    {
        private static CustomFarmManager farmManager;

        public startEventPatch(CustomFarmManager farmManager) {
            startEventPatch.farmManager = farmManager;
        }

        public static void Prefix(ref Event evt) {
            if (!farmManager.Canon) {
                Point p = farmManager.FarmHousePorch;
                if (evt.id == 1590166) {
                    evt.eventCommands[2] = "farmer 64 15 2 Marnie 65 16 0 cat " + (p.X - 1).ToString() + " " + (p.Y + 2).ToString() + " 2";
                } else if (evt.id == 897405) {
                    evt.eventCommands[2] = "farmer 64 15 2 Marnie 65 16 0 dog " + (p.X - 1).ToString() + " " + (p.Y + 2).ToString() + " 2";
                }
            }
        }
    }
}
