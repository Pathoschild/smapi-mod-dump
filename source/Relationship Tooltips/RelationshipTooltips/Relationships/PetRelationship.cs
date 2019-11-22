using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Characters;

namespace RelationshipTooltips.Relationships
{
    public class PetRelationship : IRelationship
    {
        public bool BreakAfter => false;
        public virtual int Priority => 30000;
        public virtual Func<Character, Item, bool> ConditionsMet => (c, i) => { return c is Pet; };

        public virtual string GetDisplayText<T>(string currentDisplay, T character, Item item = null) where T : Character
        {
            return "";
        }

        public virtual string GetHeaderText<T>(string currentHeader, T character, Item item = null) where T : Character
        {
            return (character as Pet).displayName;
        }
    }
}
