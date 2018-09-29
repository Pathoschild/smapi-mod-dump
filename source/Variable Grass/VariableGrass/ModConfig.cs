namespace VariableGrass
{
    /// <summary>The configuration settings.</summary>
    internal class ModConfig
    {
        /// <summary>The minimum grow iterations per day.</summary>
        public int MinIterations { get; set; }

        /// <summary>The maximum grow iterations per day.</summary>
        public int MaxIterations { get; set; } = 2;
    }
}
