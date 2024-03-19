/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/S1mmyy/StardewMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace AutoTrash
{
    class ModConfig
    {
        public bool MinesOnly { get; set; } = true;
        public IList<string> DeleteItems { get; set; } = [];
        public SButton ToggleTrash { get; set; } = SButton.RightAlt;
    }
}