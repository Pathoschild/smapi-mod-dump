/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.AdvancedLocationLoader2.Structure.Version1
{
    class TileEdit
    {
#pragma warning disable CS0649
        public string MapName;
        public int X;
        public int Y;
        public string Layer;
        public string TileSheet;
        public int? TileIndex;
        public Dictionary<string, string> Properties;
        public string Conditions;
#pragma warning restore CS0649
    }
}
