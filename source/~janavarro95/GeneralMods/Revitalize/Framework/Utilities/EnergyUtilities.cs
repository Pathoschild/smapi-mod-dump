/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Revitalize.Framework.Utilities
{
    public class EnergyUtilities
    {

        public static Color GetEnergyRemainingColor(Energy.EnergyManager Energy)
        {
            Color col = new Color();
            //ModCore.log("Energy is: " + this.energy.energyPercentRemaining);
            if (Energy.energyPercentRemaining > .75d)
            {
                col = Color.Green;
            }
            else if (Energy.energyPercentRemaining > .5d && Energy.energyPercentRemaining <= .75d)
            {
                col = Color.GreenYellow;
            }
            else if (Energy.energyPercentRemaining > .25d && Energy.energyPercentRemaining <= .5d)
            {
                col = Color.Yellow;
            }
            else if (Energy.energyPercentRemaining > .10d && Energy.energyPercentRemaining <= .25d)
            {
                col = Color.Orange;
            }
            else
            {
                col = Color.Red;
            }
            return col;
        }


    }
}
