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
