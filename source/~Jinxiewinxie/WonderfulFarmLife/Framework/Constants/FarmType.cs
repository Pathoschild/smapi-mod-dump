namespace WonderfulFarmLife.Framework.Constants
{
    /// <summary>A known farm type.</summary>
    /// <remarks>The enum value matches <see cref="StardewValley.Game1.whichFarm"/> (see <see cref="StardewValley.Farm.getMapNameFromTypeInt"/>).</remarks>
    public enum FarmType
    {
        /// <summary>The standard farm which focuses on farming.</summary>
        Standard = 0,

        /// <summary>The riverlands farm which focuses on fishing.</summary>
        Riverland = 1,

        /// <summary>The forest farm which focuses on foraging.</summary>
        Forest = 2,

        /// <summary>The hilltop farm which focuses on mining.</summary>
        Hilltop = 3,

        /// <summary>The wilderness farm which focuses on adventure.</summary>
        Wilderness = 4
    }
}