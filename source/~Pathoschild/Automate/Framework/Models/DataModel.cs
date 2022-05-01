/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Pathoschild.Stardew.Common;

namespace Pathoschild.Stardew.Automate.Framework.Models
{
    /// <summary>The model for Automate's internal file containing data that can't be derived automatically.</summary>
    internal class DataModel
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name to use for each floor ID.</summary>
        public Dictionary<int, DataModelFloor> FloorNames { get; set; } = new();

        /// <summary>Mods which add custom machine recipes and require a separate automation component.</summary>
        public DataModelIntegration[] SuggestedIntegrations { get; set; } = Array.Empty<DataModelIntegration>();

        /// <summary>The configuration for specific machines by ID.</summary>
        public Dictionary<string, ModConfigMachine> DefaultMachineOverrides { get; set; } = new();


        /*********
        ** Public methods
        *********/
        /// <summary>Normalize the model after it's deserialized.</summary>
        /// <param name="context">The deserialization context.</param>
        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            this.FloorNames ??= new();
            this.SuggestedIntegrations ??= Array.Empty<DataModelIntegration>();
            this.DefaultMachineOverrides = this.DefaultMachineOverrides.ToNonNullCaseInsensitive();
        }
    }
}
