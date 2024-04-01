/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/lenient-window-resize
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lenient_Window_Resize
{
    public class ModConfig
    {
        public int MinW { get; set; }
        public int MinH { get; set; }

        public ModConfig()
        {
            MinW = 640;
            MinH = 360;
        }

    }
}
