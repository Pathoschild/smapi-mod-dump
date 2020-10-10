/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/paritee/Paritee.StardewValley.Frameworks
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickPatchBuildings
{
    class BuildingPatch
    {
        public string Format;
        public string Type;
        public string Data;
        public string Asset;
        public bool Seasonal;

        private const byte MAGICAL_INDEX = 18;

        public string GetSeasonalAsset(string season)
        {
            List<string> parts = this.Asset.Split('/').ToList();

            parts[parts.Count - 1] = $"{parts[parts.Count - 1]}_{season}";

            return String.Join("/", parts).ToLower();
        }

        public bool IsMagical()
        {
            string[] data = this.Data.Split('/');

            if (BuildingPatch.MAGICAL_INDEX >= data.Count())
                return false;

            return Convert.ToBoolean(data[BuildingPatch.MAGICAL_INDEX]);
        }
    }
}