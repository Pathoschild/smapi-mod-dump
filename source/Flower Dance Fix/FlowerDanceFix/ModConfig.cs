/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elfuun1/FlowerDanceFix
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;
using StardewModdingAPI;
using HarmonyLib;

namespace FlowerDanceFix
{
    public class ModConfig
    {
        public ModConfig Config;

        //WIP- Changes number of pairs dancing- not tested for more than 6 pairs, hypothetically up to 15 pairs?
        public int MaxDancePairs { get; set; } = 6;

        //Pairs of NPCs dancing are random male-female pairs
        public bool NPCsHaveRandomPartners { get; set; } = false;

        //WIP- Pairs of NPCs dancing are random pairs of random genders- will require additional sprites
        //public bool ForceHeteroPartners { get; set; } = true;

        //WIP- Code to select datables that are not male of female (ie undefined; gender = 2)- will require additional sprites
        //public bool AllowNonBinaryPartners { get; set; } = false;

        //Can select datables living outside the valley (ie not town; homeRegion != 2)
        public bool AllowTouristPartners { get; set; } = false;

        //Manually match pairs
        //public string CustomMatchPairs { get; set; } = "";

        //Configureable blacklist of datables to be removed from genderedList pools- enclose NPC base name in quotes, deliniate forward slash
        public string DancerBlackList { get; set; } = "";

        //WIP- Allow custom crowd animations
        //public bool AllowCrowdAnimation { get; set; } = false;
    }
}
