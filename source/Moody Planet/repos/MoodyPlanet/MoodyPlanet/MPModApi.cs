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
