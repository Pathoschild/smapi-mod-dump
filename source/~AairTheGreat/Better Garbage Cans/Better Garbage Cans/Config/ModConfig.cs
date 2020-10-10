/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

namespace BetterGarbageCans.Config
{
    public class ModConfig
    {
        public bool enableMod { get; set; }
        public bool useCustomGarbageCanTreasure { get; set; }
        public bool allowMultipleItemsPerDay { get; set; }
        public bool allowGarbageCanRecheck { get; set; }
        public double baseChancePercent { get; set; }
        public double baseTrashChancePercent { get; set; }
        public bool enableBirthdayGiftTrash { get; set; }
        public double birthdayGiftChancePercent { get; set; }
        public int FriendshipPoints { get; set; }
        public int LinusFriendshipPoints { get; set; }
        public int WaitTimeIfFoundNothing { get; set; }
        public int WaitTimeIfFoundSomething { get; set; }
        
    }
}
