/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Collections.Generic;

namespace ChestPreview
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool ShowWhenFacing { get; set; } = true;
        public bool ShowWhenHover { get; set; } = true;
        public SButton ModKey { get; set; } = SButton.LeftAlt;
    }
}
