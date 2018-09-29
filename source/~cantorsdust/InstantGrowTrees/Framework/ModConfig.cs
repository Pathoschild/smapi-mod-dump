namespace InstantGrowTrees.Framework
{
    /// <summary>The mod configuration model.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Whether fruit trees grow instantly overnight.</summary>
        public bool FruitTreesInstantGrow { get; set; } = true;

        /// <summary>Whether fruit trees instantly age to iridium quality.</summary>
        public bool FruitTreesInstantAge { get; set; } = false;

        /// <summary>Whether non-fruit trees grow instantly overnight.</summary>
        public bool RegularTreesInstantGrow { get; set; } = true;

        /// <summary>Whether non-fruit trees grow instantly in winter.</summary>
        public bool RegularTreesGrowInWinter { get; set; }
    }
}
