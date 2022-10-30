/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Collections.Generic;

namespace TakeAll
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool TakeSameByDefault { get; set; } = false;
        public bool CloseAfterTake { get; set; } = false;
        public SButton TakeButton { get; set; } = SButton.Tab;
        public SButton ModButton { get; set; } = SButton.LeftControl;
    }
}
