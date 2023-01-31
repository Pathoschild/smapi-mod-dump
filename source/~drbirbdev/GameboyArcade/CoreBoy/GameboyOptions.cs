/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.IO;
using CoreBoy.memory.cart.battery;

namespace CoreBoy
{
    public class GameboyOptions
    {
        public FileInfo? RomFile => string.IsNullOrWhiteSpace(Rom) ? null : new FileInfo(Rom);

        public string Rom { get; set; }

        public IBattery Battery { get; set; }

        public bool ForceDmg { get; set; }

        public bool ForceCgb { get; set; }

        public bool UseBootstrap { get; set; }

        public bool DisableBatterySaves { get; set; }

        public bool Debug { get; set; }

        public bool Headless { get; set; }

        public bool Interactive { get; set; }

        public bool ShowUi => !Headless;

        public bool IsSupportBatterySaves() => !DisableBatterySaves;

        public bool RomSpecified => !string.IsNullOrWhiteSpace(Rom);

        public GameboyOptions()
        {
        }

        public void Verify()
        {
            if (ForceDmg && ForceCgb)
            {
                throw new ArgumentException("force-dmg and force-cgb options are can't be used together");
            }
        }

    }
}
