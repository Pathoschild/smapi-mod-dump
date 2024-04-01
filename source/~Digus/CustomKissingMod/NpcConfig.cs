/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomKissingMod
{
    public class NpcConfig
    {
        public string Name { get; set; }
        public int Frame { get; set; }
        public bool FrameDirectionRight { get; set; }
        public string RequiredEvent { get; set; }
    }
}
