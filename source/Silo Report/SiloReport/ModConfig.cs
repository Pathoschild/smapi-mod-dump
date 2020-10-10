/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bogie5464/SiloReport
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiloReport
{
    public enum notificationModes
    {
        //Report sent over chat
        ChatMode,

        //Report pop up in UI
        UIMode
    }
    class ModConfig
    {
        //Changes whether you want to check if you're the master player. Only applicable in the chat mode
        public bool checkMasterPlayer { get; set; } = true;

        // Changes notification mode. ChatMode for chat mode, UIMode for pop up UI notification mode.
        public notificationModes notificationMode { get; set; } = notificationModes.UIMode;
    }
}
