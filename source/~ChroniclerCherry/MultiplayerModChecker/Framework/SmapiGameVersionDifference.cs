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
    public class SmapiGameVersionDifference
    {
        public bool FarmhandHasSmapi { get; set; }
        public ISemanticVersion HostSmapiVersion { get; set; }
        public ISemanticVersion FarmhandSmapiVersion { get; set; }
    }
}
