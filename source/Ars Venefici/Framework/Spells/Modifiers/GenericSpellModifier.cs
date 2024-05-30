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
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Spells.Modifiers
{

    public class GenericSpellModifier : AbstractModifier
    {
        string id = "";

        protected Dictionary<ISpellPartStat, ISpellPartStatModifier> modifiers = new Dictionary<ISpellPartStat, ISpellPartStatModifier>();

        public override string GetId()
        {
            return id;
        }

        public void SetId(string id)
        {
            this.id = id;
        }

        public override ISpellPartStatModifier GetStatModifier(ISpellPartStat stat)
        {
            //return modifiers[stat];
            return modifiers.Where(kvp => kvp.Key.equals(stat)).ToList().Find(c=> c.Key.equals(stat)).Value;
        }

        public override List<ISpellPartStat> GetStatsModified()
        {
            return modifiers.Keys.ToList();
        }

        public GenericSpellModifier AddStatModifier(ISpellPartStat stat, ISpellPartStatModifier modifier)
        {
            modifiers.Add(stat, modifier);
            return this;
        }

        public override int ManaCost()
        {
            return 1;
        }
    }
}
