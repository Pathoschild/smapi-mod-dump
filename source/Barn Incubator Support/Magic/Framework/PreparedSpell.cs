/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using Magic.Framework.Spells;

namespace Magic.Framework
{
    internal class PreparedSpell
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The learned spell ID.</summary>
        public string SpellId { get; set; }

        /// <summary>The spell level.</summary>
        public int Level { get; set; }


        /*********
        ** Public methods
        *********/
        public PreparedSpell() { }

        public PreparedSpell(string spellId, int level)
        {
            this.SpellId = spellId;
            this.Level = level;
        }

        public PreparedSpell(Spell spell, int level)
        {
            this.SpellId = spell.ParentSchoolId + ":" + spell.Id;
            this.Level = level;
        }
    }
}
