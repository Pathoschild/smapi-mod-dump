/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace PlanImporter
{
    public class Import
    {
        public string id { get; set; } = "";
        public List<ImportTile> tiles { get; set; }
        public List<ImportTile> buildings { get; set; }
    }

    public class ImportTile
    {
        public string type { get; set; }
        public string y { get; set; }
        public string x { get; set; }
    }
}
