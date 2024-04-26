/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using MagicSkillCode.Framework.Spells;

namespace MagicSkillCode.Framework.Schools
{
    public class ArcaneSchool : School
    {
        /*********
        ** Public methods
        *********/
        public ArcaneSchool()
            : base(SchoolId.Arcane) { }

        public override Spell[] GetSpellsTier1()
        {
            return new[] { SpellManager.Get("arcane:analyze"), SpellManager.Get("arcane:magicmissle") };
        }

        public override Spell[] GetSpellsTier2()
        {
            return new[] { SpellManager.Get("arcane:disenchant"), SpellManager.Get("arcane:enchant") };
        }

        public override Spell[] GetSpellsTier3()
        {
            return new[] { SpellManager.Get("arcane:rewind") };
        }
    }
}
