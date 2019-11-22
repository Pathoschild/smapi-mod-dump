using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelationshipTooltips.Relationships
{
    public interface IRelationship
    {
        /// <summary>
        /// The result of this is used to determine if it applies to the Character under the mouse. The Character is the character under the mouse and the Item is the player's currently held item.
        /// </summary>
        Func<Character, Item, bool> ConditionsMet { get; }
        /// <summary>
        /// The header text to display/add to the tooltip. If the result is blank - it is ignored.
        /// </summary>
        /// <typeparam name="T">The specific Character derived Type being requested.</typeparam>
        /// <param name="character">The character to generate the header from.</param>
        /// <param name="item">The item assumed to be held by the player.</param>
        /// <returns>The header text based on the Relationship implementation.</returns>
        string GetHeaderText<T>(string currentHeader, T character, Item item = null) where T : Character;
        /// <summary>
        /// The body text to display on the tooltip. Blank results are ignored.
        /// </summary>
        /// <typeparam name="T">The specific Character derived Type being requested</typeparam>
        /// <param name="character">The character for which the body is being requested.</param>
        /// <param name="item">The item assumed to be held by the player.</param>
        /// <returns>The body text based on the Relationship implementation.</returns>
        string GetDisplayText<T>(string currentDisplay, T character, Item item = null) where T : Character;
        /// <summary>
        /// The priority to which this Relationship will be sorted - higher is first. Please don't use unreasonably large numbers. Identical values will be at the mercy of List.sort for ordering.
        /// </summary>
        int Priority { get; }
        /// <summary>
        /// If true, all Relationships of lower priority will be ignored.
        /// </summary>
        bool BreakAfter { get; }
    }
}
