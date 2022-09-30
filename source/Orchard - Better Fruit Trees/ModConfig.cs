/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Saitoue/Orchard
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard
{
    public class ModConfig
    {
        public bool dropSappling { get; set; } = true;
        public bool extraFruitFertilizer { get; set; } = true;
        public bool extraFruitLevel { get; set; } =true;
        public bool outOfSeasonTrees { get; set; } =true; 
        public int fruitPerLevel { get; set; } = 10;
        public bool expFromTrees { get; set; } = true;

        
    }
}
