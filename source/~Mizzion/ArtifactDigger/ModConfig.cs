/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

namespace ArtifactDigger
{
    public class ModConfig
    {
        //The dig radius.
        public int DigRadius { get; set; } = 1;

        public bool HighlightArfiactSpots { get; set; } = true;

        //Whether or not the mod should auto scan or by button press.
        public bool AutoArtifactScan { get; set; } = false;

        //Button to Scan for artifacts
        public string ArtifactScanKey { get; set; } = "Z";

        //Whether or not the mod should shake trees.
        public bool ShakeTrees { get; set; } = false;

        //Whether or not the mod should shake bushes.
        public bool ShakeBushes { get; set; } = false;
    }
}
