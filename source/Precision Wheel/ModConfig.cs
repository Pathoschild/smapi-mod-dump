/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DespairScent/PrecisionWheel
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DespairScent.PrecisionWheel
{
    internal class ModConfig
    {
        public KeybindList keybind1 = new KeybindList();
        public KeybindList keybind10 = new KeybindList(SButton.LeftShift);
        public KeybindList keybind100 = new KeybindList(SButton.LeftControl);

        public bool keybindInheritance = true;

        public bool reverseWheel = false;
    }
}
