/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSoundManager.Framework.Music
{
    /// <summary>
    /// Config class for the mod.
    /// </summary>
    public class Config
    {
        public bool EnableDebugLog;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Config()
        {
            EnableDebugLog = false;
        }

    }
}
