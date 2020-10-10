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

namespace PlatoWarpMenu
{
    public class Config
    {
        public SButton MenuButton { get; set; }

        public bool UseTempFolder { get; set; }

        public string MenuFont1 { get; set; }

        public string MenuFont2 { get; set; }

        public bool CompatibilityMode { get => true; set { } }

        public Config()
        {
            MenuButton = SButton.J;
            UseTempFolder = Constants.TargetPlatform == GamePlatform.Android;
            MenuFont1 = "opensans";
            MenuFont2 = "escrita";
            CompatibilityMode = Constants.TargetPlatform == GamePlatform.Android;
        }
    }
}
