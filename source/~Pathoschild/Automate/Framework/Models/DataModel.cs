/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Pathoschild.Stardew.Common;

namespace Pathoschild.Stardew.Automate.Framework.Models
{
    /// <summary>The model for Automate's internal file containing data that can't be derived automatically.</summary>
    internal class DataModel
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Mods which add custom machine recipes and require a separate automation component.</summary>
        public DataModelIntegration[] SuggestedIntegrations { get; }

        /// <summary>The configuration for specific machines by ID.</summary>
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Auto)]
        public Dictionary<string, ModConfigMachine> DefaultMachineOverrides { get; } = new(StringComparer.OrdinalIgnoreCase);


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="suggestedIntegrations">Mods which add custom machine recipes and require a separate automation component.</param>
        public DataModel(DataModelIntegration[]? suggestedIntegrations)
        {
            this.SuggestedIntegrations = suggestedIntegrations ?? [];
            this.DefaultMachineOverrides = this.DefaultMachineOverrides.ToNonNullCaseInsensitive();
        }
    }
}
