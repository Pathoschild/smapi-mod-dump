/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/Pravoloxinone
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pravoloxinone
{
    internal class ModConfig
    {
        public float BuffChance { get; set; } = 0.65f;
        public float DebuffChance { get; set; } = 0.2f;
        public float DamageChance { get; set; } = 0.10f;
        public float DeathChance { get; set; } = 0.05f;

    }
}
