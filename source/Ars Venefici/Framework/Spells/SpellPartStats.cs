/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Interfaces.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Spells
{
    public class SpellPartStats : ISpellPartStat
    {

        public SpellPartStatType spellPartStatType;
        private string id;

        public SpellPartStats(SpellPartStatType type)
        {
            spellPartStatType = type;
            id = spellPartStatType.ToString().ToLower();
        }

        public string GetId()
        {
            return id;
        }

        public void SetSpellPartStatType(SpellPartStatType type)
        {
            spellPartStatType = type;
        }
    }

    public enum SpellPartStatType
    {
        BOUNCE,
        DAMAGE,
        DURATION,
        HEALING,
        PIERCING,
        POWER,
        RANGE,
        SPEED
    }
}
