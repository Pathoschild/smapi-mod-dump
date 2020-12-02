/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassProduction
{
    public class StaticValues
    {
        /// <summary>
        /// All machines that need support outside of PFM-enabled ones.
        /// </summary>
        public static readonly Dictionary<string, InputRequirement> SUPPORTED_VANILLA_MACHINES = new Dictionary<string, InputRequirement>()
        {
            { "Seed Maker", InputRequirement.InputRequired },
            { "Worm Bin", InputRequirement.NoInputsOnly },
        };
    }
}
