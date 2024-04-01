/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace UniqueValley
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public string ForbiddenList { get; set; } = "George";
        public bool MaintainGender { get; set; } = true;
        public bool MaintainAge { get; set; } = true;
        public bool MaintainDatable { get; set; } = true;
        public bool RandomizeGiftTastes { get; set; } = false;
    }
}
