/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace Pathoschild.Stardew.Automate.Framework.Models
{
    /// <summary>The model for Automate's internal file containing data that can't be derived automatically.</summary>
    internal class DataModel
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name to use for each floor ID.</summary>
        public Dictionary<int, string> FloorNames { get; set; }

        /// <summary>Mods which add custom machine recipes and require a separate automation component.</summary>
        public DataModelIntegration[] SuggestedIntegrations { get; set; }

        /// <summary>The configuration for specific machines by ID.</summary>
        public IDictionary<string, ModConfigMachine> DefaultMachineOverrides { get; set; }
    }
}
