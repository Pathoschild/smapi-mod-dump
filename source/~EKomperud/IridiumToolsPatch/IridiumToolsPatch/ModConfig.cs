/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EKomperud/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IridiumToolsPatch
{
    public class ModConfig
    {
        public int length;
        public int radius;

        public ModConfig()
        {
            length = 5;
            radius = 2;
        }
    }
}
