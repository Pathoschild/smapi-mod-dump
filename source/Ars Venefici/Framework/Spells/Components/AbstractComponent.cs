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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Spells.Components
{
    public abstract class AbstractComponent : ISpellComponent
    {
        private HashSet<ISpellPartStat> stats;

        protected AbstractComponent(params ISpellPartStat[] stats)
        {
            this.stats = stats.ToHashSet();
        }

        public abstract string GetId();

        public virtual HashSet<ISpellPartStat> GetStatsUsed()
        {
            return stats;
        }

        public abstract SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, CharacterHitResult target, int index, int ticksUsed);

        public abstract SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, TerrainFeatureHitResult target, int index, int ticksUsed);
        public abstract int ManaCost();
    }
}
