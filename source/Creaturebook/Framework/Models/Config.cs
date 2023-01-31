/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KediDili/Creaturebook
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace Creaturebook.Framework.Models
{
    public class ModConfig
    {
        public bool ShowScientificNames { get; set; } = true;
        public bool ShowDiscoveryDates { get; set; } = true;
        public KeybindList OpenMenuKeybind { get; set; } = KeybindList.Parse("LeftControl + LeftShift + B");
        public string WayToGetNotebook { get; set; } = "Letter";
        public bool EnableStickies { get; set; } = true;
    }
}
