/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FairfieldBW/CropCheck
**
*************************************************/

using System.Collections.Generic;

namespace CropCheck
{
    class ModData
    {
        public IDictionary<string, Dictionary<string, string>> TileChecks { get; set; } = new Dictionary<string, Dictionary<string, string>>();

        public ModData()
        {
            this.TileChecks.Add("Farm", new Dictionary<string, string>());
            this.TileChecks.Add("IslandWest", new Dictionary<string, string>());
            this.TileChecks.Add("Greenhouse", new Dictionary<string, string>());
        }
    }
}
