/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;

namespace MultiplayerModChecker.Framework
{
    public class ModVersions
    {
        public string ModName { get; set; }

        public string ModUniqueId { get; set; }
        public bool DoesHostHave { get; set; } = false;
        public bool DoesFarmhandHave { get; set; } = false;
        public ISemanticVersion HostModVersion { get; set; }
        public ISemanticVersion FarmhandModVersion { get; set; }
    }
}
