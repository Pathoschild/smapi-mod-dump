/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/RyanJesky/IncreaseCropGrowthPhase
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace IncreaseCropGrowthPhase.Framework
{
    internal class KeyBindConfig
    {
        //get and set keybind from config.json
        public KeybindList GrowCropsKey { get; set; } = new KeybindList();

    }
}
