/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace InstantLoad
{
    public class Config { 

        public bool EnableInstantLoad { get; set; } = true;

        public bool LoadHost { get; set; } = false;

        public bool EnableDebugCommands { get; set; } = true;

        public List<DebugTrigger> DebugCommands { get; set; } = new List<DebugTrigger>();

    }
}
