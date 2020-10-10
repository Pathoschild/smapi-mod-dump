/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;

namespace Portraiture
{
    class PConfig
    {
        public SButton changeKey { get; set; } = SButton.P;
        public SButton menuKey { get; set; } = SButton.M;
        public string active { get; set; } = "none";
    }
}
