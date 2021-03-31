/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/SkullCavernToggle
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace SkullCavernToggle
{
    internal class ModConfig
    {      
        public bool ShrineToggle { get; set; } = true;
        public KeybindList ToggleDifficulty { get; set; } = KeybindList.Parse("Z");
        public bool MustCompleteQuest { get; set; } = true;
    }
}
