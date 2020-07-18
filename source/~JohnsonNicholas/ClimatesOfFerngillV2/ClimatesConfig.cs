using StardewModdingAPI;

namespace TwilightShards.ClimatesOfFerngillV2
{
    public class ClimatesConfig
    {
        /// <summary>This controls the verbosity of the debug. </summary>
        public bool Verbose { get; set; } = true;

        /// <summary>This sets what key sets up the menu.  </summary>
        public SButton WeatherMenuToggle { get; set; } = SButton.Z;

        /// <summary>The number of pixels to shift content on each up/down scroll.</summary>
        public int ScrollAmount { get; set; } = 160;

        /// <summary>The climate file the mod reads base weather from </summary>
        public string Climate { get; set; } = "normal.json";

        /// <summary>This controls events that can inflict harm on the player, in various ways.</summary>
        public bool HazardousEvents { get; set; } = true;

        /// <summary>The threshold for frost, set to 2.22 C or 36 F by default. </summary>
        /// <remarks>Adjusting this adjusts the chances for frost.</remarks>
        public float FrostThreshold { get; set; } = 2.22f;

        /// <summary> The threshold for wilting, set to 33C or 91.4F </summary>
        /// <remarks> Adjusting this adjusts the chance for heatwaves.</remarks>
        public float WiltThreshold { get; set; } = 33f;
    }
}
