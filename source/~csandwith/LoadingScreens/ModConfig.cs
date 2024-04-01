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

namespace LoadingScreens
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool ShowOnWarp { get; set; } = false; public bool MustLikeItem { get; set; } = true;
    }
}
