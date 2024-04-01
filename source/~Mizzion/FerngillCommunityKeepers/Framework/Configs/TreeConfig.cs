/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerngillCommunityKeepers.Framework.Configs
{
    internal class TreeConfig
    {
        public bool CutDownTrees { get; set; } = false;

        public bool CutDownSaplings { get; set; } = false;

        public bool DigUpTreeSeeds { get; set; } = false;
    }
}
