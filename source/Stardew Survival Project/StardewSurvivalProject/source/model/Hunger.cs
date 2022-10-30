/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewSurvivalProject.source.model
{
    public class Hunger
    {
        public static double DEFAULT_VALUE = ModConfig.GetInstance().MaxHunger;   
        public double value { get; set; }

        public Hunger()
        {
            this.value = DEFAULT_VALUE;
        }

    }
}
