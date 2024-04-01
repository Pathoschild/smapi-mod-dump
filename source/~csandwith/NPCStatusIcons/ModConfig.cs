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

namespace NPCStatusIcons
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool RequireModKey { get; set; } = true;
        public bool ShowGiftable { get; set; } = true;
        public bool ShowTalkable { get; set; } = true;
        public bool ShowBirthday { get; set; } = true;
        public SButton ModKey { get; set; } = SButton.LeftAlt;
    }
}
