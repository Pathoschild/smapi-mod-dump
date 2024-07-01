/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArsVenefici.Framework.Skill;
using System.Reflection;
using System.Collections.ObjectModel;

namespace ArsVenefici.Framework.Interfaces.Spells
{
    public interface ISpellPartSkillHelper
    {
        bool Knows(ModEntry modEntry, Farmer player, string skillID);

        bool Knows(ModEntry modEntry, Farmer player, SpellPartSkill skill);

        bool CanLearn(ModEntry modEntry, Farmer player, SpellPartSkill skill);

        void Learn(ModEntry modEntry, Farmer player, SpellPartSkill skill);

        void Forget(ModEntry modEntry, Farmer player, SpellPartSkill skill);

        void LearnAll(ModEntry modEntry, Farmer player);

        void ForgetAll(ModEntry modEntry, Farmer player);

        IDictionary<string, SpellPartSkill> GetKnownSpellPartSkills(ModEntry modEntry, Farmer player);
    }
}
