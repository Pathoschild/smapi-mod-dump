/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KabigonFirst/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace MineAssist.Config {
    public class CmdCfg {
        public SButton modifyKey { get; set; }
        public SButton key { get; set; }
        public string cmd { get; set; }
        public Dictionary<string, string> par { get; set; }

        public CmdCfg(SButton mKey, SButton key, string cmd, Dictionary<string, string> par) {
            this.modifyKey = mKey;
            this.key = key;
            this.cmd = cmd;
            this.par = par;
        }
    }
}