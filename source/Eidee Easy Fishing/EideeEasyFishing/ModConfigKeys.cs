/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/eideehi/EideeEasyFishing
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;

namespace EideeEasyFishing
{
    internal class ModConfigKeys
    {
        public SButton ReloadConfig { get; }

        public ModConfigKeys(SButton reloadConfig)
        {
            this.ReloadConfig = reloadConfig;
        }
    }
}
