/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using ItemResearchSpawner.Models.Enums;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ItemResearchSpawner
{
    public class ModConfig
    {
        public KeybindList ShowMenuKey { get; set; } = KeybindList.ForSingle(SButton.R);
        public ModMode DefaultMode { get; set; } = ModMode.Spawn;
        public bool UseDefaultConfig { get; set; } = true;
    }
}