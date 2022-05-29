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
using Newtonsoft.Json;

namespace Pathoschild.Stardew.ChestsAnywhere.Framework
{
    /// <summary>The model for the <c>data.json</c> file.</summary>
    internal class ModData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The predefined world areas for <see cref="ChestRange.CurrentWorldArea"/>.</summary>
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Auto)]
        public IDictionary<string, HashSet<string>> WorldAreas { get; } = new Dictionary<string, HashSet<string>>();
    }
}
