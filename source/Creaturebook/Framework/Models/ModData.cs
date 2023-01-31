/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KediDili/Creaturebook
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI.Utilities;

namespace Creaturebook.Framework.Models
{
    public class ModData
    {
        public IDictionary<string, SDate> DiscoveryDates { get; set; } = new Dictionary<string, SDate>();
        public bool IsNotebookObtained { get; set; } = false;
    }
}
