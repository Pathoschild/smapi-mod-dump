/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using StardewModdingAPI;

namespace QuickLoad
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public SButton Hotkey { get; set; } = SButton.F7;
        public string SaveFolder { get; set; }
        public bool UseLastLoaded { get; set; } = true;
    }
}
