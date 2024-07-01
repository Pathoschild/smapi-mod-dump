/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;
using Pathoschild.Stardew.Common;

namespace Pathoschild.Stardew.DataLayers.Framework
{
    /// <summary>Configures the settings for each data layer.</summary>
    internal class ModConfigLayers
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Settings for the accessible layer.</summary>
        public LayerConfig Accessible { get; set; } = new() { UpdatesPerSecond = 2 };

        /// <summary>Settings for the buildable layer.</summary>
        public LayerConfig Buildable { get; set; } = new() { UpdatesPerSecond = 2 };

        /// <summary>Settings for the bee house layer.</summary>
        public LayerConfig CoverageForBeeHouses { get; set; } = new() { UpdatesPerSecond = 60 };

        /// <summary>Settings for the Junimo hut layer.</summary>
        public LayerConfig CoverageForJunimoHuts { get; set; } = new() { UpdatesPerSecond = 60 };

        /// <summary>Settings for the scarecrow layer.</summary>
        public LayerConfig CoverageForScarecrows { get; set; } = new() { UpdatesPerSecond = 60 };

        /// <summary>Settings for the sprinkler layer.</summary>
        public LayerConfig CoverageForSprinklers { get; set; } = new() { UpdatesPerSecond = 60 };

        /// <summary>Settings for the fertilizer layer.</summary>
        public LayerConfig CropFertilizer { get; set; } = new() { UpdatesPerSecond = 30 };

        /// <summary>Settings for the crop harvest layer.</summary>
        public LayerConfig CropHarvest { get; set; } = new() { UpdatesPerSecond = 2 };

        /// <summary>Settings for the crop water layer.</summary>
        public LayerConfig CropWater { get; set; } = new() { UpdatesPerSecond = 30 };

        /// <summary>Settings for the crop paddy water layer.</summary>
        public LayerConfig CropPaddyWater { get; set; } = new() { UpdatesPerSecond = 30 };

        /// <summary>Settings for the machine processing layer.</summary>
        public LayerConfig Machines { get; set; } = new() { UpdatesPerSecond = 2 };

        /// <summary>Settings for the tile grid layer.</summary>
        public LayerConfig TileGrid { get; set; } = new() { UpdatesPerSecond = 1 };

        /// <summary>Settings for the tillable layer.</summary>
        public LayerConfig Tillable { get; set; } = new() { UpdatesPerSecond = 2 };


        /*********
        ** Public methods
        *********/
        /// <summary>Normalize the model after it's deserialized.</summary>
        /// <param name="context">The deserialization context.</param>
        [OnDeserialized]
        [SuppressMessage("ReSharper", "NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract", Justification = SuppressReasons.MethodValidatesNullability)]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = SuppressReasons.UsedViaOnDeserialized)]
        public void OnDeserialized(StreamingContext context)
        {
            this.Accessible ??= new LayerConfig { UpdatesPerSecond = 2 };
            this.Buildable ??= new LayerConfig { UpdatesPerSecond = 2 };
            this.CoverageForBeeHouses ??= new LayerConfig { UpdatesPerSecond = 60 };
            this.CoverageForJunimoHuts ??= new LayerConfig { UpdatesPerSecond = 60 };
            this.CoverageForScarecrows ??= new LayerConfig { UpdatesPerSecond = 60 };
            this.CoverageForSprinklers ??= new LayerConfig { UpdatesPerSecond = 60 };
            this.CropFertilizer ??= new LayerConfig { UpdatesPerSecond = 30 };
            this.CropHarvest ??= new LayerConfig { UpdatesPerSecond = 2 };
            this.CropWater ??= new LayerConfig { UpdatesPerSecond = 30 };
            this.CropPaddyWater ??= new LayerConfig { UpdatesPerSecond = 30 };
            this.Machines ??= new LayerConfig { UpdatesPerSecond = 2 };
            this.TileGrid ??= new LayerConfig { UpdatesPerSecond = 1 };
            this.Tillable ??= new LayerConfig { UpdatesPerSecond = 2 };
        }

        /// <summary>Get whether any layers are enabled.</summary>
        public bool AnyLayersEnabled()
        {
            foreach (PropertyInfo property in typeof(ModConfigLayers).GetProperties())
            {
                if (property.GetValue(this) is LayerConfig { Enabled: true })
                    return true;
            }

            return false;
        }
    }
}
