/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley;

namespace ExpControl.Core
{
    [HarmonyPatch(typeof(Farmer), nameof(Farmer.gainExperience))]
    class GainExperience_patch
    {
        [HarmonyLib.HarmonyPrefix]
        private static void Prefix(ref int which, ref int howMuch)
        {
            switch (which)
            {
                case 0:
                    //farming
                    howMuch = (int)(ModEntry.Config.FarmingEXP * howMuch);
                    break;
                case 3:
                    //mining
                    howMuch = (int)(ModEntry.Config.MiningEXP * howMuch);
                    break;
                case 1:
                    //fishing
                    howMuch = (int)(ModEntry.Config.FishingEXP * howMuch);
                    break;
                case 2:
                    //forage
                    howMuch = (int)(ModEntry.Config.ForagingEXP * howMuch);
                    break;
                case 4:
                    //combat
                    howMuch = (int)(ModEntry.Config.CombatEXP * howMuch);
                    break;
            }

        }
    }
}
