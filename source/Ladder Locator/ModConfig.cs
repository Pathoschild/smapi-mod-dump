/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/yuri-moens/LadderLocator
**
*************************************************/

using StardewModdingAPI;
using System;

namespace LadderLocator
{
    class ModConfig
    {
        public SButton ToggleShaftsKey { get; set; } = SButton.OemTilde;
        public bool ForceShafts { get; set; } = false;
    }

}
