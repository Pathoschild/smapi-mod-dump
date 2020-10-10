/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JessebotX/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprint_Sprint.Framework.Config
{
    class StaminaDrainConfig
    {
        /// <summary> Check if stamina draining is on </summary>
        public bool Enabled { get; set; } = true;
        /// <summary> How much stamina lost every second </summary>
        public float StaminaCost { get; set; } = 1f;
    }
}
