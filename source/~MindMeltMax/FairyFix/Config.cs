/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairyFix
{
    public class Config
    {
        public SelectionMode SelectMode { get; set; } = SelectionMode.ConnectedSameCrop;

        public bool ReviveDeadCrops { get; set; } = true;

        public bool ResetOnSeasonChange { get; set; } = true;

        public KeybindList ToggleButton { get; set; } = new(SButton.U);
    }
}
