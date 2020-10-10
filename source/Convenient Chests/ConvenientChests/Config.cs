/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using StardewModdingAPI;

namespace ConvenientChests
{
    public class Config
    {
        public bool CategorizeChests { get; set; } = true;
        public bool StashToExistingStacks { get; set; } = true;

        public bool StashToNearbyChests { get; set; } = true;
        public int StashRadius { get; set; } = 5;
        public SButton StashKey { get; set; } = SButton.Q;
        public SButton? StashButton { get; set; } = SButton.RightStick;

        public bool StashAnywhere { get; set; } = false;
        public bool StashAnywhereToExistingStacks { get; set; } = false;
        public bool StashAnywhereToFridge { get; set; } = true;
        public SButton StashAnywhereKey { get; set; } = SButton.Z;

        public bool CraftFromChests { get; set; } = true;
        public int CraftRadius { get; set; } = 5;
    }
}
