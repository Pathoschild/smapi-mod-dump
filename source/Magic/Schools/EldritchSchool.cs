/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/Magic
**
*************************************************/

using Magic.Spells;

namespace Magic.Schools
{
    internal class EldritchSchool : School
    {
        public EldritchSchool() : base( SchoolId.Eldritch )
        {
        }

        public override Spell[] GetSpellsTier1()
        {
            return new Spell[] { SpellBook.get("eldritch:meteor"), SpellBook.get("eldritch:bloodmana") };
        }

        public override Spell[] GetSpellsTier2()
        {
            return new Spell[] { SpellBook.get("eldritch:lucksteal") };
        }

        public override Spell[] GetSpellsTier3()
        {
            return new Spell[] { SpellBook.get("eldritch:spirit") };
        }
    }
}