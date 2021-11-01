/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace XSLite.Migrations.JsonAsset
{
    using System.Collections.Generic;

    internal record JsonAsset
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int ReserveExtraIndexCount { get; set; } = 0;

        public Recipe Recipe { get; set; } = null;

        public string PurchaseFrom { get; set; } = null;

        public int PurchasePrice { get; set; } = 0;

        public Dictionary<string, string> NameLocalization { get; set; } = null;

        public Dictionary<string, string> DescriptionLocalization { get; set; } = null;
    }
}