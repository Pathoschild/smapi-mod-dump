/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/M3ales/RelationshipTooltips
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace RelationshipTooltips.Relationships
{
    public class PlayerRelationship : IRelationship
    {
        public bool BreakAfter => false;
        public virtual int Priority => 60000;
        public virtual Func<Character, Item, bool> ConditionsMet => (c, i) => { return c is Farmer && c != Game1.player; };

        public virtual string GetDisplayText<T>(string currentDisplay, T character, Item item = null) where T : Character
        {
            if (character == Game1.MasterPlayer && character != Game1.player)
                return "The Farm owner.";
            return "Another Farmhand.";
        }

        public virtual string GetHeaderText<T>(string currentHeader, T character, Item item = null) where T : Character
        {
            return character.displayName;
        }
    }
}
