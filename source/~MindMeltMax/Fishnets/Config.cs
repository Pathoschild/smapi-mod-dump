/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fishnets
{
    internal class Config
    {
        public int TextureVariant { get; set; } = 0;

        public bool LessTrash { get; set; } = false;

        public bool LessWeeds { get; set; } = false;

        public bool LessJelly { get; set; } = false;
    }
}
