/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;

namespace NoCooties
{
    internal class PPaFModConfig
    {
        public string PlatonicNPCs { get; set; }
    }
    
    internal class ModConfig
    {
        internal List<string> knownNPCs;

        public ModConfig()
        {
            knownNPCs = cslToList("Abigail, Alex, Elliott, Emily, Haley, Harvey, Leah, Maru, Penny, Sam, Sebastian, Shane");
            HuggingNPCs = new List<string>(knownNPCs);
        }

        internal List<string> cslToList(string csl)
        {
            return csl.Split(',')
                .Select(item => item.Trim())
                .ToList();
        }
        
        public bool EndlessHugs { get; set; } = true;
        public bool CustomNPCsAreHuggers { get; set; } = true;
        public List<string> HuggingNPCs { get; set; }

        internal bool IsHugger(string npc)
        {
            if (!knownNPCs.Contains(npc)) return CustomNPCsAreHuggers;
            return HuggingNPCs.Contains(npc);
        }
    }
}