using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Monsters;

namespace RelationshipTooltips.Relationships
{
    public class HorseRelationship : IRelationship
    {
        public Func<Character, Item, bool> ConditionsMet => (c, i) => { return c is Horse; };

        public int Priority => -30000;

        public bool BreakAfter => false;

        public string GetDisplayText<T>(string currentDisplay, T character, Item item = null) where T : Character
        {
            return "";
        }

        public string GetHeaderText<T>(string currentHeader, T character, Item item = null) where T : Character
        {
            return character.displayName;
        }
    }
}
