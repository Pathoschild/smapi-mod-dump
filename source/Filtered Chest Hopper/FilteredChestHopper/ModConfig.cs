/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shivion/StardewValleyMods
**
*************************************************/


using GenericModConfigMenu;

namespace FilteredChestHopper
{
    internal class ModConfig
    {
        public bool CompareQuality { get; set; } = false;
        public bool CompareQuantity { get; set; } = false;
        public int TransferInterval { get; set; } = 60;
    }
}
