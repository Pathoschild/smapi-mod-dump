/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/F1r3w477/TheseModsAintLoyal
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;

namespace MoodyPlanet
{
    public class MPModApi
    {
        ModEntry ME;

        public MPModApi(ModEntry me)
        {
            ME = me;
        }
        public double Exp_Rate()
        {
            return ME.CMS[3];
        }

    }
}
