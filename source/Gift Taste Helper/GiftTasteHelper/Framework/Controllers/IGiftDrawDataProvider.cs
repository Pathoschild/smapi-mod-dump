namespace GiftTasteHelper.Framework
{
    /// <summary>Provides the data used in drawing gifts for an npc.</summary>
    internal interface IGiftDrawDataProvider
    {
        /// <summary>Returns true if gift data exists for this npc.</summary>
        bool HasDataForNpc(string npcName);

        /// <summary>Gets the gift draw data for an npc.</summary>
        /// <param name="npcName">The name of the npc to fetch the gifts for.</param>
        /// <param name="tastesToDisplay">The tastes of the gifts to include.</param>
        /// <param name="includeUniversal">Should universal gifts be included.</param>
        GiftDrawData GetDrawData(string npcName, GiftTaste[] tastesToDisplay, bool includeUniversal);
    }
}
