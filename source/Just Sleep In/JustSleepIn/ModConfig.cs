/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/9Rifleman/JustSleepIn
**
*************************************************/

using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustSleepIn
{
    public class ModConfig
    {
        public KeybindList StaticTimeToggle { get; set; } = KeybindList.Parse("LeftControl + OemTilde");
    }
}
