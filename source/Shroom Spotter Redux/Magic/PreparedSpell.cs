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

namespace Magic
{
    public class PreparedSpell
    {
        public string SpellId { get; set; }
        public int Level { get; set; }

        public PreparedSpell() { }

        public PreparedSpell(string spellId, int level)
        {
            SpellId = spellId;
            Level = level;
        }

        public PreparedSpell( Spell spell, int level )
        {
            SpellId = spell.ParentSchoolId + ":" + spell.Id;
            Level = level;
        }
    }
}
