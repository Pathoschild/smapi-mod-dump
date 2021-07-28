/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace ObjectTimeLeft.Framework
{
    internal class Configuration
    {
        public SButton ToggleKey { get; set; } = SButton.L;
        public float TextScale { get; set; } = 1.0f;
        public bool ShowOnStart { get; set; } = true;
    }
}
