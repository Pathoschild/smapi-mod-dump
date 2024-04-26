/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/BetterMeowmere
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterMeowmere
{
    public class ModConfig
    {
        public string ProjectileSound { get; set; } = "All";
        public bool ProjectileIsSecondaryAttack { get; set; } = true;
        public int AttackDamage { get; set; } = 20;
    }
}
