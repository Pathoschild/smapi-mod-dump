namespace ArtifactDigger
{
    public class ModConfig
    {
        //The dig radius.
        public int DigRadius { get; set; } = 1;

        public bool HighlightArfiactSpots { get; set; } = true;

        //Whether or not the mod should auto scan or by button press.
        //public bool AutoArtifactScan { get; set; } = false;

        //Button to Scan for artifacts
        public string ArtifactScanKey { get; set; } = "Z";

        //Whether or not the mod should shake trees.
        public bool ShakeTrees { get; set; } = false;

        //Whether or not the mod should shake bushes.
        public bool ShakeBushes { get; set; } = false;
    }
}
