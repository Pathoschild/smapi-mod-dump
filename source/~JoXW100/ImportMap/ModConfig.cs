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
using StardewModdingAPI.Utilities;

namespace ImportMap
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public KeybindList ImportKey { get; set; } = KeybindList.Parse("LeftShift + F12");

    }
}
