/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

namespace MultiplayerModChecker.Framework
{
    public class Config
    {
        public string[] IgnoredMods { get; set; } = { "Cherry.MultiplayerModChecker" };
        public bool HideReportInTrace { get; set; } = false;
    }
}
