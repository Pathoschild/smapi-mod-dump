/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;

namespace ModUpdater
{
    public class Config
    {
        public bool AutoRestart { get; set; } = false;

        public string ExecutionArgs { get; set; } = "";

        public DateTime LastUpdateCheck { get; set; } = new DateTime();

        public int Interval { get; set; } = 60;

        public bool LoadPrereleases { get; set; } = false;

        public string GitHubUser { get; set; } = "";

        public string GitHubPassword { get; set; } = "";

        public List<string> Exclude { get; set; } = new List<string>();
     }
}
