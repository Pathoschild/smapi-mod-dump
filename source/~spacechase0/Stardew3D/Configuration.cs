/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace Stardew3D
{
    public class Configuration
    {
        public int MultisampleCount { get; set; } = 0;

        public KeybindList RotateLeft { get; set; } = new KeybindList(SButton.Q);
        public KeybindList RotateRight { get; set; } = new KeybindList(SButton.R);
    }
}
