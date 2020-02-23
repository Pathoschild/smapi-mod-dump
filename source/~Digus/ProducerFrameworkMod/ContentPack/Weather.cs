namespace ProducerFrameworkMod.ContentPack
{
    /// <summary>An in-game weather. (Copied from Content Patcher)</summary>
    public enum Weather
    {
        /// <summary>The weather is sunny (including festival/wedding days). This is the default weather if no other value applies.</summary>
        Sunny,
        /// <summary>Rain is falling, but without lightning.</summary>
        Rainy,
        /// <summary>Rain is falling with lightning.</summary>
        Stormy,
        /// <summary>Snow is falling.</summary>
        Snowy,
        /// <summary>The wind is blowing with visible debris (e.g. flower petals in spring and leaves in fall).</summary>
        Windy
    }
}