/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-MisophoniaAccessibility
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MisophoniaAccessibility.Config
{
    public class ModConfig
    {
        /// <summary>
        /// Sets whether this mod is enabled, default true
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Object containing which sounds should be disabled
        /// </summary>
        public Sounds SoundsToDisable { get; set; } = new Sounds();
    }
}
