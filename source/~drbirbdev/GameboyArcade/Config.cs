/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using BirbShared.Config;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace GameboyArcade
{
    [ConfigClass(I18NNameSuffix = "")]
    class Config
    {

        [ConfigOption]
        public SButton Up { get; set; } = SButton.W;

        [ConfigOption]
        public SButton Down { get; set; } = SButton.S;

        [ConfigOption]
        public SButton Left { get; set; } = SButton.A;

        [ConfigOption]
        public SButton Right { get; set; } = SButton.D;

        [ConfigOption]
        public SButton A { get; set; } = SButton.MouseLeft;

        [ConfigOption]
        public SButton B { get; set; } = SButton.MouseRight;

        [ConfigOption]
        public SButton Start { get; set; } = SButton.Space;

        [ConfigOption]
        public SButton Select { get; set; } = SButton.Tab;

        [ConfigOption]
        public SButton Power { get; set; } = SButton.Escape;

        [ConfigOption]
        public SButton Turbo { get; set; } = SButton.F1;
    }
}
