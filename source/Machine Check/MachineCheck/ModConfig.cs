/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FairfieldBW/MachineCheck
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace MachineCheck
{
    class ModConfig
    {
        public KeybindList ToggleKey { get; set; } = KeybindList.Parse("J");
        public bool AutoUnsave { get; set; } = false;
    }
}
