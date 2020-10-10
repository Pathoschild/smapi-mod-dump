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
using StardewValley.Characters;

namespace RelationshipTooltips.Relationships
{
    public class EasterEgg : PetRelationship
    {
        public override int Priority => 40000;
        public override Func<Character, Item, bool> ConditionsMet => (c, i) => { return base.ConditionsMet(c,i) && c is Cat && c.displayName.Trim() == "drakkensong"; };

        public override string GetDisplayText<T>(string currentDisplay, T character, Item item = null)
        {
            return "(Not dead)";
        }

        public override string GetHeaderText<T>(string currentHeader, T character, Item item = null)
        {
            return "";
        }
    }
}
