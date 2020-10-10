/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SourceZh/MultiplayerIdle
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerIdle {
    class ModConfig {
        public string IdleMethod { get; set; }
        public int IdleCheckSeconds { get; set; }
        public ModConfig() {
            IdleMethod = "SINGLE";
            IdleCheckSeconds = 3;
        }
    }
}
