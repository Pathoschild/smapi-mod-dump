/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

namespace Randomizer
{
    /// <summary>
    /// The indexes in the Data/NPCDIspositions.xnb dictionary
    /// Add to this enum as these are used
    /// </summary>
    public enum NPCDispositionIndexes
    {
        Birthday = 8
    }

    /// <summary>
    /// The indexes in the Data/NPCGiftTastes.xnb dictionary
    /// Add to this enum as these are used
    /// </summary>
    public enum NPCGiftTasteIndexes
    {
        Loves = 1,
        Likes = 3,
        Dislikes = 5,
        Hates = 7,
        Neutral = 9
    }

    /// <summary>
    /// This is an internal enum mapping indexes to the values in 
    /// PreferenceRandomizer.UniversalPreferenceKeys
    /// </summary>
    public enum UniversalPreferencesIndexes
    {
        Loved,
        Liked,
        Neutral,
        Disliked,
        Hated
    }

    /// <summary>
    /// This is an internal enum mapping indexes to the values in 
    /// PreferenceRandomizer.GiftableNPCs
    /// </summary>
    public enum GiftableNPCIndexes
    {
        Robin,
        Demetrius,
        Maru,
        Sebastian,
        Linus,
        Pierre,
        Caroline,
        Abigail,
        Alex,
        George,
        Evelyn,
        Lewis,
        Clint,
        Penny,
        Pam,
        Emily,
        Haley,
        Jas,
        Vincent,
        Jodi,
        Kent,
        Sam,
        Leah,
        Shane,
        Marnie,
        Elliott,
        Gus,
        Dwarf,
        Wizard,
        Harvey,
        Sandy,
        Willy,
        Krobus,
        Leo
    };
}
