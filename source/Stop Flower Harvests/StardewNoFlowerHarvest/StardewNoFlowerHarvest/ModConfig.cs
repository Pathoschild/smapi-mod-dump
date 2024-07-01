/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/seichen/Stardew-Stop-Flower-Harvests
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Utilities;

namespace StopFlowerHarvests
{
    public sealed class ModConfig
    {
        public bool FairyRose { get; set; } = true;
        public bool Poppy { get; set; } = true;
        public bool BlueJazz { get; set; } = true;
    }
}
