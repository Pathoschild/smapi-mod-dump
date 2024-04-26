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

namespace Screenshot
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;

        public SButton ScreenshotKey { get; set; } = SButton.F9;
        public string ScreenshotFolder { get; set; } = "Screenshots";
        public string Message { get; set; } = "Screenshot saved to {0}";
    }
}
