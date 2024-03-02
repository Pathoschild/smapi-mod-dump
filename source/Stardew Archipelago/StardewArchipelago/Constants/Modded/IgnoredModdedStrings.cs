/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;

namespace StardewArchipelago.Constants.Modded
{
    public class IgnoredModdedStrings
    {
        // Just a repository of lists and dictionaries in some hope to keep it out of vanilla code.
        public static readonly List<string> FriendsNotMetImmediately = new(){
            "Yoba", "Lance", "Apples", "Scarlett", "Morgan", "Zic", "Alecto", "Gunther",
            "Marlon", "Gregory"
        };
        
        public static readonly List<string> Shipments = new(){
            "Galmoran Gem", "Ancient Hilt", "Ancient Blade", "Ancient Doll Legs", "Ancient Doll Body", "Prismatic Shard Piece 3", 
            "Mask Piece 1", "Mask Piece 2", "Mask Piece 3", "Prismatic Shard Piece 1", "Prismatic Shard Piece 2", "Prismatic Shard Piece 4", 
            "Chipped Amphora Piece 1", "Chipped Amphora Piece 2", 
        };

        public static readonly List<string> Quests = new(){
            "Transgressions"
        };

        public static readonly List<string> SpecialOrders = new(){
            "Aurora Vineyard", "Monster Crops"
        };

        public static readonly List<string> Craftables = new(){
            "Restore Prismatic Shard", "Restore Golden Mask", "Restore Ancient Sword", "Restore Ancient Doll", "Restore Chipped Amphora"
        };

    }
}