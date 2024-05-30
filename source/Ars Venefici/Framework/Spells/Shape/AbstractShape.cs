/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Interfaces;
using ArsVenefici.Framework.Interfaces.Spells;
using ArsVenefici.Framework.Util;
using Newtonsoft.Json;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Spells.Shape
{
    public abstract class AbstractShape : ISpellShape
    {
        private HashSet<ISpellPartStat> stats;

        public AbstractShape(params ISpellPartStat[] stats)
        {
            this.stats = new HashSet<ISpellPartStat>(stats);
        }

        public abstract string GetId();

        public abstract SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, HitResult hit, int ticksUsed, int index, bool awardXp);

        public virtual HashSet<ISpellPartStat> GetStatsUsed()
        {
            return stats;
        }

        public virtual bool IsContinuous()
        {
            return false;
        }

        public virtual bool IsEndShape()
        {
            return false;
        }

        public virtual bool NeedsPrecedingShape()
        {
            return false;
        }

        public virtual bool NeedsToComeFirst()
        {
            return false;
        }

        public abstract int ManaCost();
    }
}
