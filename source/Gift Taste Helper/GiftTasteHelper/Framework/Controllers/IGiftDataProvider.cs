using System.Collections.Generic;

namespace GiftTasteHelper.Framework
{
    internal interface IGiftDataProvider
    {
        // Invoked when the data is changed.
        event DataSourceChangedDelegate DataSourceChanged;

        /// <summary>Gets all gifts of a particular taste for an npc.</summary>
        /// <param name="npcName">The name of the npc to fetch the info for.</param>
        /// <param name="taste">The taste of gifts to get.</param>
        /// <param name="includeUniversal">Should universal gifts be included.</param>
        /// <returns>A list of item/category Ids.</returns>
        IEnumerable<int> GetGifts(string npcName, GiftTaste taste, bool includeUniversal = false);

        /// <summary>Gets all the universal gifts of a particular taste for an npc.</summary>
        /// <param name="npcName">The name of the npc to fetch the info for.</param>
        /// <param name="taste">The taste of gifts to get.</param>
        /// <returns>A list of item/category Ids.</returns>
        IEnumerable<int> GetUniversalGifts(string npcName, GiftTaste taste);
    }
}
