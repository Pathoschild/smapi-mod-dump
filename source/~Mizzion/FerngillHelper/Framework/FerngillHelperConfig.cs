/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace FerngillHelper.Framework
{
    internal class FerngillHelperConfig
    {
        public bool ShowTreeInfo { get; set; } = true;

        public bool ShowCropInfo { get; set; } = true;

        public bool ShowMachineInfo { get; set; } = true;

        public bool AllowTeleport { get; set; } = true;

        public SButton TeleportKey { get; set; } = SButton.F3;

        public bool UseHoverMode { get; set; } = true;
    }
}
