namespace SkullCaveSaver.Framework
{
    /// <summary>The mod configuration model.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The last saved Skull Cave floor.</summary>
        public int LastMineLevel { get; set; }

        /// <summary>The multiple of floors at which to save (e.g. <c>5</c> to save every fifth floor).</summary>
        public int SaveLevelEveryXFloors { get; set; } = 5;
    }
}
