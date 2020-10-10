/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JessebotX/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprint.Config
{
    class SprintConfig
    {
        /// <summary> The player's sprinting speed. </summary>
        public int SprintSpeed { get; set; } = 5;

        /// <summary> Add extra speed if 'SprintKey = LeftShift'. </summary>
        public int LeftShiftKeybindExtraSpeed { get; set; } = 7;
    }
}
