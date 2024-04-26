/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaceShaft
{
    public class ModConfig
    {
        public string ShaftCost { get; set; } = "390 200";
        public string SkillReq { get; set; } = "Mining 6";
        public int PercentDamage { get; set; } = 100;
        public int PercentLevels { get; set; } = 100;
        public bool PreventGoingToSkullCave { get; set; } = true;
        public bool SkipConfirmOnShaftJump { get; set; } = false;
    }
}
