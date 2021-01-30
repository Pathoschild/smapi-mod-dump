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

namespace MoodyPlanetEvolution
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
            return ME.mood.modifiers[3];
        }

        public double Health_Multi()
        {
            return ME.mood.modifiers[0];
        }

        public double Resilience_Multi()
        {
            return ME.mood.modifiers[1];
        }

        public double Slipperiness_Multi()
        {
            return ME.mood.modifiers[2];
        }

        public double Scale_Multi()
        {
            return ME.mood.modifiers[3];
        }

        public double Speed_Multi()
        {
            return ME.mood.modifiers[4];
        }

        public List<int> changedMons()
        {
            return ME.blackhash;
        }
        
        public List<int> virginMons()
        {
            return ME.hashofm;
        }

    }
}
