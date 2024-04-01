/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jpparajeles/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildFlowersReimagined
{
    
    public sealed class SaveDataItem
    {
        public int Vector2X { get; set; }
        public int Vector2Y { get; set;}

        public int PhaseToShow { get; set; }
        public int CurrentPhase {  get; set; }
        public string? SeedIndex { get; set; }
        public byte TintColorR { get; set; }
        public byte TintColorG { get; set; }
        public byte TintColorB { get; set; }
        public byte TintColorA { get; set; }
        public bool Dead { get; set; }


    }

    public sealed class SaveData
    {
        public Dictionary<string, List<SaveDataItem>> PatchMapData { get; set; }
    }
}
