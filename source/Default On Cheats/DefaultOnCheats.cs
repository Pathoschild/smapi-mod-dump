/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EnderTedi/DefaultOnCheats
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultOnCheats
{
    public class DefaultOnCheats : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Program.enableCheats = true;
        }
    }
}
