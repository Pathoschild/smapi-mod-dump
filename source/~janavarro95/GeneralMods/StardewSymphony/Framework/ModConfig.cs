namespace Omegasis.StardewSymphony.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /// <summary>The minimum delay (in milliseconds) to pass before playing the next song, or 0 for no delay.</summary>
        public int MinSongDelay { get; set; } = 10000;

        /// <summary>The maximum delay (in milliseconds) to pass before playing the next song, or 0 for no delay.</summary>
        public int MaxSongDelay { get; set; } = 30000;

        /// <summary>Whether to disable ambient rain audio when music is playing. If false, plays ambient rain audio alongside whatever songs are set in rain music.</summary>
        public bool SilentRain { get; set; }

        /// <summary>Whether to play seasonal music from the music packs, instead of defaulting to the Stardew Valley Soundtrack.</summary>
        public bool PlaySeasonalMusic { get; set; } = true;
    }
}
