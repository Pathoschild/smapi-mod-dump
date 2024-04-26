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
    public class DebugTrigger
    {
        public string Target { get; set; } = "Console";

        public string Event { get; set; } = "Load";

        public string Command { get; set; }

        public List<string> Args { get; set; } = new List<string>();
    }
}
