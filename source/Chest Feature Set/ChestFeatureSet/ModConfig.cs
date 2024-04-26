/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack20136/ChestFeatureSet
**
*************************************************/

using StardewModdingAPI;

namespace ChestFeatureSet
{
    public sealed class ModConfig
    {
        // StashToChests
        public bool StashToChests { get; set; } = true;
        public bool OnlyStashToExistingStacks { get; set; } = false;
        public string StashLocationSetting { get; set; } = "Default";
        public int StashRadius { get; set; } = 32;
        public SButton StashKey { get; set; } = SButton.B;

        // LockItems
        public bool LockItems { get; set; } = true;
        public SButton LockItemKey { get; set; } = SButton.LeftAlt;
        public SButton ResetLockItemKey { get; set; } = SButton.RightAlt;

        // CraftFromChests
        public bool CraftFromChests { get; set; } = true;
        public string CraftLocationSetting { get; set; } = "Default";
        public int CraftRadius { get; set; } = -1;
        public SButton OpenCraftingPageKey { get; set; } = SButton.K;

        // MoveChests
        public bool MoveChests { get; set; } = true;
        public bool MoveChestsDebuff { get; set; } = true;
        public SButton MoveChestKey { get; set; } = SButton.LeftShift;

        // Variables
        public string[] CustomArea { get; set; } = { "Custom_GrandpasShed", "Custom_GrandpasShedGreenhouse" };
    }
}
