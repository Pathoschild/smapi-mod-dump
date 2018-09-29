namespace WonderfulFarmLife.Framework.Config
{
    /// <summary>Defines a group of tile overrides to apply.</summary>
    internal class LayoutConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>A human-readable name for the layout (e.g. displayed in errors).</summary>
        public string Name { get; set; }

        /// <summary>A boolean setting in the <c>config.json</c> that must be true for this layout to be applied (if any).</summary>
        public string ConfigFlag { get; set; }

        /// <summary>The tile overrides to apply.</summary>
        public TileConfig[] Tiles { get; set; }

        /// <summary>The tile properties to set.</summary>
        public TilePropertyConfig[] TileProperties { get; set; }

        /// <summary>The tile properties to set.</summary>
        public TileIndexPropertyConfig[] TileIndexProperties { get; set; }
    }
}
