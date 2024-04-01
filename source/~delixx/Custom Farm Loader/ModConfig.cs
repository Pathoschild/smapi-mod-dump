/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/custom-farm-loader
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Farm_Loader
{
    public class ModConfig
    {
        public float LoadMenuIconScale { get; set; }
        public float CoopMenuIconScale { get; set; }
        
        public bool IncludeVanilla { get; set; }
        public bool DisableStartFurniture { get; set; }

        public ModConfig()
        {
            LoadMenuIconScale = 1f;
            CoopMenuIconScale = 1f;
            IncludeVanilla = false;
            DisableStartFurniture = false;
        }

    }
}
