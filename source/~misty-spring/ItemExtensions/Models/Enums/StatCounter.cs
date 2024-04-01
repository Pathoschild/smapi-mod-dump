/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

namespace ItemExtensions.Models.Enums;

/// <summary>
/// What stat to count the resource towards.
/// See <see cref="StardewValley.Stats"/> for possible types
/// </summary>
public enum StatCounter
{
    None,
    Copper,             //Found
    Diamonds,
    GeodesBroken,
    Gold,
    Iridium,
    Iron,
    MysticStones,       //Crushed
    OtherGems,          //:OtherPreciousGemsFound
    PrismaticShards,    //Found
    Stone,              //Gathered
    Stumps,             //Chopped
    Seeds,              //Sown
    Weeds,              //Eliminated
    Any                 //ONLY USED FOR ITEMQUERY   
}