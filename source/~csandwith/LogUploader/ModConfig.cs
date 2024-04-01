/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Collections.Generic;

namespace LogUploader
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool OpenBehind { get; set; } = false;
        public bool SendOnFatalError { get; set; } = true;
        public bool ShowButton { get; set; } = true;
        public SButton SendButton { get; set; } = SButton.F15;
        public SButton ShowButtonButton { get; set; } = SButton.LeftAlt;

    }
}
