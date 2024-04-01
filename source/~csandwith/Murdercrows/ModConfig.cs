/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murdercrows
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public float MaxDistanceSpawn { get; set; } = 25;
        public int MaxSimultaneousMonsters { get; set; } = 20;
        public bool EnableMonsterWaves { get; set; } = true;
    }
}
