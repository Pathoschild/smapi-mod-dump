/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FairfieldBW/MachineCheck
**
*************************************************/

using System.Collections.Generic;

namespace MachineCheck
{
    class ModData
    {
        public IDictionary<string, Dictionary<string, string>> MachineChecks { get; set; } = new Dictionary<string, Dictionary<string, string>>();
    }
}
