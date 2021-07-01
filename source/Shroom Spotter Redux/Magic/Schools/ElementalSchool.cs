/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using Magic.Spells;

namespace Magic.Schools
{
    internal class ElementalSchool : School
    {
        public ElementalSchool() : base( SchoolId.Elemental )
        {
        }

        public override Spell[] GetSpellsTier1()
        {
            return new Spell[] { SpellBook.get("elemental:fireball"), SpellBook.get("elemental:frostbolt") };
        }

        public override Spell[] GetSpellsTier2()
        {
            return new Spell[] { SpellBook.get("elemental:descend") };
        }

        public override Spell[] GetSpellsTier3()
        {
            return new Spell[] { SpellBook.get("elemental:teleport") };
        }
    }
}
