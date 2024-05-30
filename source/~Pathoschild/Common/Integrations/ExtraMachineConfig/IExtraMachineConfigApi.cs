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
using StardewValley.GameData.Machines;

namespace Pathoschild.Stardew.Common.Integrations.ExtraMachineConfig
{
    /// <summary>The API provided by the Extra Machine Config mod.</summary>
    public interface IExtraMachineConfigApi
    {
        /// <summary>Retrieves the extra fuels consumed by this recipe.</summary>
        /// <param name="outputData">The output rule to check.</param>
        IList<(string, int)> GetExtraRequirements(MachineItemOutput outputData);

        /// <summary>Retrieves the extra tag-defined fuels consumed by this recipe.</summary>
        /// <param name="outputData">The output rule to check.</param>
        IList<(string, int)> GetExtraTagsRequirements(MachineItemOutput outputData);
    }
}
